using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;


public class WebSocketClient : MonoBehaviour
{
    private ClientWebSocket _ws;
    private CancellationTokenSource _cancellation;
    public Action<string> OnNotificationReceived;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public async Task Connect(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("WebSocket connection failed: userId is empty");
            UnityMainThreadDispatcher.Instance().Enqueue(() => OnNotificationReceived?.Invoke("Connection failed: userId is empty"));
            return;
        }

        _ws = new ClientWebSocket();
        _cancellation = new CancellationTokenSource();

        try
        {
            Debug.Log($"Connecting to WebSocket: wss://mge-server.onrender.com/ws?userId={userId}");
            await _ws.ConnectAsync(new Uri($"wss://mge-server.onrender.com/ws?userId={userId}"), _cancellation.Token);
            Debug.Log("WebSocket connected successfully");
            UnityMainThreadDispatcher.Instance().Enqueue(() => OnNotificationReceived?.Invoke("WebSocket connected successfully"));

            // Переключення на ігрову сцену після успішного підключення
            UnityMainThreadDispatcher.Instance().Enqueue(() => UnityEngine.SceneManagement.SceneManager.LoadScene("MenuScene")); // Виконуємо в головному потоці

            // Запускаємо асинхронне отримання повідомлень
            await ReceiveMessages();
        }
        catch (Exception ex)
        {
            Debug.LogError($"WebSocket connection failed: {ex.Message}");
            UnityMainThreadDispatcher.Instance().Enqueue(() => OnNotificationReceived?.Invoke($"Connection failed: {ex.Message}"));
        }
    }

    private async Task ReceiveMessages()
    {
        var buffer = new byte[1024 * 4];
        while (_ws.State == WebSocketState.Open)
        {
            try
            {
                var result = await _ws.ReceiveAsync(new ArraySegment<byte>(buffer), _cancellation.Token);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Debug.Log($"Received: {message}");
                    UnityMainThreadDispatcher.Instance().Enqueue(() => OnNotificationReceived?.Invoke(message));
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    Debug.Log("WebSocket closed by server");
                    UnityMainThreadDispatcher.Instance().Enqueue(() => OnNotificationReceived?.Invoke("WebSocket closed by server"));
                    await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed", _cancellation.Token);
                    break;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"WebSocket receive error: {ex.Message}");
                UnityMainThreadDispatcher.Instance().Enqueue(() => OnNotificationReceived?.Invoke($"Receive error: {ex.Message}"));
                break;
            }
        }
    }

    public async Task SendMessage(string message)
    {
        if (_ws == null || _ws.State != WebSocketState.Open)
        {
            Debug.LogError("WebSocket is not connected");
            return;
        }

        try
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            await _ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, _cancellation.Token);
            Debug.Log($"Sent: {message}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"WebSocket send error: {ex.Message}");
        }
    }

    private void OnDestroy()
    {
        _cancellation?.Cancel();
        _ws?.Dispose();
        Debug.Log("WebSocketClient destroyed");
    }
}
public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static UnityMainThreadDispatcher _instance;
    private static readonly System.Collections.Concurrent.ConcurrentQueue<Action> _executionQueue = new();

    public void Awake()
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
        if (_instance == null)
        {
            var go = new GameObject("UnityMainThreadDispatcher");
            _instance = go.AddComponent<UnityMainThreadDispatcher>();
        }
        return _instance;
    }
}