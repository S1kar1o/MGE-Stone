using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
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
       Fighting,
    }

    public TurnState currentState;

    private List<TurnState> turnQueue = new List<TurnState>();
    private int currentIndex = 0;
    private bool playerFirst;
    private bool isPlayerFirstSet = false;
    public static TurnManager Instance { get; private set; }
    public Button EndTurnButton;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // якщо вже є інший екземпляр, видаляємо цей
            return;
        }
        Instance = this;
    }
    private void Start()
    {
        EndTurnButton.onClick.AddListener(() =>
        {
            StartNextTurn();
        });
        InitializeTurnOrder();
        StartNextTurn();
    }

    private void InitializeTurnOrder()
    {
        turnQueue.Clear();

        // Рандомізуємо, хто ходить першим
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

    public void StartNextTurn()
    {
        if (turnQueue.Count == 0) {
            InitializeTurnOrder();
            currentIndex = 0;
        };

        currentState = turnQueue[currentIndex];

        // Після закінчення поточного ходу викликаємо наступний
        TurnChanged?.Invoke(this, new OnStateChangedEventArgs { state = currentState });

        currentIndex = (currentIndex + 1) % turnQueue.Count;
    }
}
