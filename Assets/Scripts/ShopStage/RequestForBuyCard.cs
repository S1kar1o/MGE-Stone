using System;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static LoadAvailbleCards;
public class RequestForBuyCard : MonoBehaviour
{
    private const string baseUrl = "https://mge-server.onrender.com";

    [SerializeField] private string userId;
    private Button buyOneObjectButton;
    private Button buyTenObjectButton;
    [SerializeField] private RewardProcessing rewardSoloSummonProcessing;
    [SerializeField] private RewardProcessing rewardTenSummonProcessing;
    private void Start()
    {
        userId = LoginLogic.instance.GetUserId();
    }

    public void SetUpButtons(Button forSoloRoll,Button forx10Roll)
    {
        buyOneObjectButton= forSoloRoll;
        buyTenObjectButton= forx10Roll;
        buyOneObjectButton.onClick.AddListener(() =>
              {
                  BuyCard((response, error) =>
                  {
                      if (string.IsNullOrEmpty(error))
                      {
                          Debug.Log($"Карту успішно отримано: {response.card[0].name}");
                          rewardSoloSummonProcessing.StartrocecingUnits(response.card.ToList());
                          rewardTenSummonProcessing.gameObject.SetActive(false);
                          rewardSoloSummonProcessing.gameObject.SetActive(true);
                      }
                      else
                      {
                          Debug.LogError($"Помилка: {error}");
                      }
                  }, "generate");
              });

        buyTenObjectButton.onClick.AddListener(() =>
        {
            BuyCard((response, error) =>
            {
                if (string.IsNullOrEmpty(error))
                {
                    rewardTenSummonProcessing.StartrocecingUnits(response.card.ToList());
                    rewardTenSummonProcessing.gameObject.SetActive(true);
                    rewardSoloSummonProcessing.gameObject.SetActive(false);
                }
                else
                {
                    Debug.LogError($"Помилка: {error}");
                }
            }, "generateTenCards");
        });
    }

    public void BuyCard(Action<GenerateCardResponse, string> callback, string corner)
    {
        if (string.IsNullOrEmpty(userId))
        {
            callback?.Invoke(null, "User ID is missing");
            return;
        }

        StartCoroutine(
            SendRequest($"{baseUrl}/cards/{corner}/{userId}", "{}", callback)
        );
    }

    private System.Collections.IEnumerator SendRequest(string url, string jsonData, Action<GenerateCardResponse, string> callback)
    {
        using (var request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            Debug.Log(request.downloadHandler.text);
            if (request.result != UnityWebRequest.Result.Success)
            {
                string errorMessage = $"HTTP Error: {request.responseCode}, {request.error}";
                if (!string.IsNullOrEmpty(request.downloadHandler.text))
                {
                    errorMessage += $", Response: {request.downloadHandler.text}";
                }
                callback?.Invoke(null, errorMessage);
                yield break;
            }

            try
            {
                // Unity JsonUtility вимагає, щоб поля в JSON точно збігалися з полями класу (Success != success)
                var response = JsonUtility.FromJson<GenerateCardResponse>(request.downloadHandler.text);
                callback?.Invoke(response, null);
            }
            catch (Exception ex)
            {
                callback?.Invoke(null, $"Помилка десеріалізації: {ex.Message}");
            }
        }
    }

    [System.Serializable]
    public class GenerateCardResponse
    {
        public bool success;
        public string message;
        public CardObject[] card;
    }
}