using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class PhotonConector : MonoBehaviourPunCallbacks
{
    [SerializeField] private string lobbySceneName = "MenuScene"; // Назва сцени лобі
    [SerializeField] private string playableSceneName = "PlaybleScene"; // Назва сцени гри
    private bool isRoomFull = false;
    public static PhotonConector instance;
    private void Awake()
    {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject); }
        else
            Destroy(gameObject);
    }
    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        // Підключення до Photon
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("Починаємо підключення до Photon...");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Підключено до Master сервера, шукаю матч...");
        // Приєднуємося до випадкової кімнати
        
    }
    public void StartBatle()
    {
        Debug.Log(1212);
        PhotonNetwork.JoinRandomRoom();
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"Не знайшов існуючої кімнати: {message}. Створюю нову...");
        // Створюємо нову кімнату з максимум 2 гравцями
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 2,
            IsVisible = true, // Кімната видима для інших
            IsOpen = true     // Кімната відкрита для приєднання
        };
        PhotonNetwork.CreateRoom(null, roomOptions);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Приєднався до кімнати: {PhotonNetwork.CurrentRoom.Name}. Гравців: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}");

        // Перевіряємо кількість гравців у кімнаті
        CheckRoomStatus();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Новий гравець приєднався: {newPlayer.NickName}. Гравців: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}");

        // Перевіряємо, чи кімната заповнена
        CheckRoomStatus();
    }

    private void CheckRoomStatus()
    {
        // Якщо в кімнаті 2 гравці, починаємо гру
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            isRoomFull = true;
            Debug.Log("Кімната заповнена! Починаємо гру...");

            // Тільки MasterClient завантажує сцену для всіх гравців
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false; // Закриваємо кімнату для нових гравців
                PhotonNetwork.LoadLevel(playableSceneName);
            }
        }
        else
        {
            Debug.Log($"Чекаємо другого гравця... ({PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers})");
            // Залишаємося в поточній сцені (лобі)
        }
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Покинув кімнату. Повертаємося до лобі...");
        UnityEngine.SceneManagement.SceneManager.LoadScene(lobbySceneName);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogError($"Відключено від Photon: {cause}");
        UnityEngine.SceneManagement.SceneManager.LoadScene(lobbySceneName);
    }

   /* private void OnDestroy()
    {
        // Відключаємося від Photon при знищенні об'єкта
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
    }*/
}