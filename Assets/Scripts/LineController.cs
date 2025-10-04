using UnityEngine;
using Photon.Pun;
using System.Threading.Tasks;
using static TurnManager;

public class LineController : MonoBehaviourPunCallbacks
{
    private PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        if (photonView == null)
        {
            photonView = gameObject.AddComponent<PhotonView>();
            photonView.ViewID = PhotonNetwork.AllocateViewID(PhotonNetwork.LocalPlayer.ActorNumber);
        }
    }

    private void Start()
    {
        TurnManager.Instance.TurnChanged += OnTurnChanged;
    }

    private async void OnTurnChanged(object sender, TurnManager.OnStateChangedEventArgs e)
    {
        if (e.state != TurnState.Fighting || !PhotonNetwork.IsMasterClient) return;

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
        TurnManager.Instance.photonView.RPC("StartNextTurn", RpcTarget.All);
    }
}