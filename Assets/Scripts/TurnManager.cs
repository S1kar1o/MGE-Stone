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

    private List<TurnState> turnQueue = new List<TurnState>();
    private int currentIndex = 0;
    private bool playerFirst;
    private bool isPlayerFirstSet = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
       
    }

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            InitializeTurnOrder();
            photonView.RPC("SyncTurnOrder", RpcTarget.All, playerFirst);
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
    }

   

    [PunRPC]
    public void EndGame(bool player1Won)
    {
        Debug.Log($"{(player1Won ? "Player 1" : "Player 2")} won!");
        PhotonNetwork.LoadLevel("LobbyScene");
    }
}