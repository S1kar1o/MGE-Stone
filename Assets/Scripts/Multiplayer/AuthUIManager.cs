using UnityEngine;
using UnityEngine.UI;

public class AuthUIManager : MonoBehaviour
{
    [SerializeField] private LoginLogic loginLogic;
    [SerializeField] private WebSocketClient webSocketClient;
    [SerializeField] private Text statusText;

    public async void OnRegisterButtonClick(string email, string password, string username)
    {
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
                //            statusText.text = $"Ошибка регистрации: {error ?? "Неизвестная ошибка"}";

                Debug.LogError($"Ошибка регистрации: {error ?? "Неизвестная ошибка"}");
                return;
            }

            //         statusText.text = $"Регистрация успешна! ID: {response.id}";

            await webSocketClient.Connect(response.id);
        });
    }

    public async void OnLoginButtonClick(string email, string password)
    {
        Debug.Log($"Login button clicked. Email: {email}, Password: {password}");

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            //            statusText.text = "Ошибка: Email и пароль должны быть заполнены";

            Debug.LogError("Email and password must be filled");
            return;
        }

        loginLogic.Login(email, password, async (response, error) =>
        {
            if (error != null || response == null)
            {
                //                statusText.text = $"Ошибка входа: {error ?? "Неизвестная ошибка"}";
                Debug.LogError($"Ошибка входа: {error ?? "Неизвестная ошибка"}");
                return;
            }

            //            statusText.text = $"Вход успешен! ID: {response.id}";
            await webSocketClient.Connect(response.id);
        });
    }
}