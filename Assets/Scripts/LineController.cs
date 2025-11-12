using UnityEngine;
using Photon.Pun;
using System.Threading.Tasks;
using static TurnManager;

public class LineController : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        // Підписуємося на подію ТІЛЬКИ на MasterClient
        if (PhotonNetwork.IsMasterClient)
        {
            TurnManager.Instance.TurnChanged += OnTurnChanged;
        }
    }

    private async void OnTurnChanged(object sender, TurnManager.OnStateChangedEventArgs e)
    {
        if (e.state != TurnState.Fighting) return;

        // Повідомляємо ВСІХ клієнтів (включно з Master) почати бій
        photonView.RPC(nameof(StartFightRPC), RpcTarget.All);
    }

    [PunRPC]
    private async void StartFightRPC()
    {
        // Цей код виконується на ВСІХ клієнтах синхронно
        foreach (Transform child in transform)
        {
            LineScript lineScript = child.GetComponent<LineScript>();
            if (lineScript != null)
            {
                await lineScript.StartUseSkills();
                await lineScript.StartAttack();
            }
        }

        // Тільки MasterClient переходить до наступного ходу
        if (PhotonNetwork.IsMasterClient)
        {
            await Task.Yield(); // Дозволяємо завершити всі await
            TurnManager.Instance.photonView.RPC("StartNextTurn", RpcTarget.All);
        }
    }

    // Опціонально: відписатися при знищенні
    private void OnDestroy()
    {
        if (TurnManager.Instance != null && PhotonNetwork.IsMasterClient)
        {
            TurnManager.Instance.TurnChanged -= OnTurnChanged;
        }
    }
}