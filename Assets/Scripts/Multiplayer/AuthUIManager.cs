using UnityEngine;
using UnityEngine.UI;

public class AuthUIManager : MonoBehaviour
{
    [SerializeField] private LoginLogic loginLogic;
    [SerializeField] private WebSocketClient webSocketClient;
    [SerializeField] private Text statusText;

    private void Start()
    {
/*        webSocketClient.OnNotificationReceived += (msg) => statusText.text = $"Уведомление: {msg}";
*/        Debug.Log("AuthUIManager initialized");
    }

    public async void OnRegisterButtonClick(string email, string password, string username)
    {
        Debug.Log($"Register button clicked. Email: {email}, Username: {username}, Password: {password}");

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(username))
        {
            statusText.text = "Ошибка: Все поля должны быть заполнены";
            Debug.LogError("All fields must be filled");
            return;
        }

        loginLogic.Register(email, password, username, async (response, error) =>
        {
            if (error != null || response == null)
            {
/*                statusText.text = $"Ошибка регистрации: {error ?? "Неизвестная ошибка"}";
*/                Debug.LogError($"Ошибка регистрации: {error ?? "Неизвестная ошибка"}");
                return;
            }

/*            statusText.text = $"Регистрация успешна! ID: {response.id}";
*/            Debug.Log($"Регистрация успешна! ID: {response.id}");
            PlayerPrefs.SetString("AccessToken", response.accessToken);

            // Асинхронне підключення до WebSocket
            await webSocketClient.Connect(response.id);
        });
    }

    public async void OnLoginButtonClick(string email, string password)
    {
        Debug.Log($"Login button clicked. Email: {email}, Password: {password}");

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
/*            statusText.text = "Ошибка: Email и пароль должны быть заполнены";
*/            Debug.LogError("Email and password must be filled");
            return;
        }

        loginLogic.Login(email, password, async (response, error) =>
        {
            if (error != null || response == null)
            {
/*                statusText.text = $"Ошибка входа: {error ?? "Неизвестная ошибка"}";
*/                Debug.LogError($"Ошибка входа: {error ?? "Неизвестная ошибка"}");
                return;
            }

/*            statusText.text = $"Вход успешен! ID: {response.id}";
*/            Debug.Log($"Вход успешен! ID: {response.id}");
            PlayerPrefs.SetString("AccessToken", response.accessToken);
            await webSocketClient.Connect(response.id);
        });
    }
}