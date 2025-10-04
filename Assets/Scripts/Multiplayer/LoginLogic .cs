using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class LoginLogic : MonoBehaviour
{
    private const string baseUrl = "https://mge-server.onrender.com";

    [Serializable]
    public class RegisterRequest
    {
        public string email;
        public string password;
        public string username;
    }

    [Serializable]
    public class LoginRequest
    {
        public string email;
        public string password;
    }

    [Serializable]
    public class AuthResponse
    {
        public string id;
        public string username;
        public string email;
        public string accessToken;
    }

    public void Register(string email, string password, string username, Action<AuthResponse, string> callback)
    {
        var requestData = new RegisterRequest { email = email, password = password, username = username };
        string jsonData = JsonUtility.ToJson(requestData);
        Debug.Log($"Отправляемый JSON (Register): {jsonData}");

        StartCoroutine(SendRequest($"{baseUrl}/auth/registrate", jsonData, callback));
    }

    public void Login(string email, string password, Action<AuthResponse, string> callback)
    {
        var requestData = new LoginRequest { email = email, password = password };
        string jsonData = JsonUtility.ToJson(requestData);
        Debug.Log($"Отправляемый JSON (Login): {jsonData}");

        StartCoroutine(SendRequest($"{baseUrl}/auth/login", jsonData, callback));
    }

    private System.Collections.IEnumerator SendRequest(string url, string jsonData, Action<AuthResponse, string> callback)
    {
        using (var request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                string errorMessage = $"HTTP Error: {request.responseCode}, {request.error}";
                if (!string.IsNullOrEmpty(request.downloadHandler.text))
                {
                    errorMessage += $", Response: {request.downloadHandler.text}";
                }
                Debug.LogError(errorMessage);
                callback(null, errorMessage);
                yield break;
            }

            try
            {
                var response = JsonUtility.FromJson<AuthResponse>(request.downloadHandler.text);
                if (response == null)
                {
                    Debug.LogError("Не вдалося десеріалізувати відповідь сервера");
                    callback(null, "Не вдалося десеріалізувати відповідь сервера");
                    yield break;
                }
                Debug.Log($"Response: {request.downloadHandler.text}");
                callback(response, null);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Помилка десеріалізації: {ex.Message}");
                callback(null, $"Помилка десеріалізації: {ex.Message}");
            }
        }
    }
}