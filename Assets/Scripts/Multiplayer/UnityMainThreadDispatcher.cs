using System;
using UnityEngine;

public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static UnityMainThreadDispatcher _instance;
    private static readonly System.Collections.Concurrent.ConcurrentQueue<Action> _executionQueue = new();

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void Update()
    {
        while (_executionQueue.TryDequeue(out var action))
        {
            action?.Invoke();
        }
    }

    public void Enqueue(Action action)
    {
        _executionQueue.Enqueue(action);
    }

    public static UnityMainThreadDispatcher Instance()
    {
        return _instance;
    }
    private void OnDestroy()
    {
        _instance = null;
        Destroy(gameObject);
    }

}
