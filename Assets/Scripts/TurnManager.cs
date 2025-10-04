using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections.Generic;

public class TurnManager : MonoBehaviourPunCallbacks
{
    public event EventHandler<OnStateChangedEventArgs> TurnChanged;
    public class OnStateChangedEventArgs : EventArgs
    {
        public TurnState state;
    }

    public enum TurnState
    {
        CalculatingTurn,
        YourSpawning,
        EnemySpawning,
        Fighting
    }

    public static TurnManager Instance { get; private set; }
    public TurnState currentState;
    [SerializeField] private Button EndTurnButton;
    [SerializeField] private float turnTime = 30f;

    private List<TurnState> turnQueue = new List<TurnState>();
    private int currentIndex = 0;
    private bool playerFirst;
    private bool isPlayerFirstSet = false;
    private PhotonView photonView;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        photonView = GetComponent<PhotonView>();
        if (photonView == null)
        {
            photonView = gameObject.AddComponent<PhotonView>();
            photonView.ViewID = PhotonNetwork.AllocateViewID(PhotonNetwork.LocalPlayer.ActorNumber);
        }
    }

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            InitializeTurnOrder();
            photonView.RPC("SyncTurnOrder", RpcTarget.All, playerFirst);
            StartCoroutine(TurnTimer());
        }
        EndTurnButton.onClick.AddListener(() =>
        {
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("StartNextTurn", RpcTarget.All);
            }
        });
    }

    private void InitializeTurnOrder()
    {
        if (!isPlayerFirstSet)
        {
            playerFirst = UnityEngine.Random.value > 0.5f;
            isPlayerFirstSet = true;
        }
        if (playerFirst)
        {
            turnQueue.Add(TurnState.YourSpawning);
            turnQueue.Add(TurnState.EnemySpawning);
            turnQueue.Add(TurnState.Fighting);
        }
        else
        {
            turnQueue.Add(TurnState.EnemySpawning);
            turnQueue.Add(TurnState.YourSpawning);
            turnQueue.Add(TurnState.Fighting);
        }
    }

    [PunRPC]
    private void SyncTurnOrder(bool isPlayerFirst)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            playerFirst = !isPlayerFirst;
            isPlayerFirstSet=true;
            InitializeTurnOrder();
        }
        StartNextTurn();
    }

    [PunRPC]
    public void StartNextTurn()
    {
        currentState = turnQueue[currentIndex];
        TurnChanged?.Invoke(this, new OnStateChangedEventArgs { state = currentState });

        currentIndex = (currentIndex + 1) % turnQueue.Count;

        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(TurnTimer());
        }
    }

    private IEnumerator TurnTimer()
    {
        yield return new WaitForSeconds(turnTime);
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("StartNextTurn", RpcTarget.All);
        }
    }

    [PunRPC]
    public void EndGame(bool player1Won)
    {
        Debug.Log($"{(player1Won ? "Player 1" : "Player 2")} won!");
        PhotonNetwork.LoadLevel("LobbyScene");
    }
}