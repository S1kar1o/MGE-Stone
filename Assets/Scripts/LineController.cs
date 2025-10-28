using UnityEngine;
using Photon.Pun;
using System.Threading.Tasks;
using static TurnManager;

public class LineController : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        TurnManager.Instance.TurnChanged += OnTurnChanged;
    }
    private async void OnTurnChanged(object sender, TurnManager.OnStateChangedEventArgs e)
    {
        if (e.state != TurnState.Fighting) return;

        foreach (Transform child in transform)
        {
            LineScript lineScript = child.GetComponent<LineScript>();
            if (lineScript != null)
            {
                await lineScript.StartUseSkills();
                await lineScript.StartAttack();
            }
        }

        // Викликаємо наступний хід після завершення атак
        await Task.Yield();
        if (PhotonNetwork.IsMasterClient)
            TurnManager.Instance.photonView.RPC("StartNextTurn", RpcTarget.All);
    }
}