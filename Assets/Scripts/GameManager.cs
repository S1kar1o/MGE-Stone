using UnityEngine;
using Photon.Pun;
using System.Collections;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance { get; private set; }

    [SerializeField]private Transform fatherForHeroes;
    [SerializeField] private Vector2 heroTransform;          // Локальний герой (знизу)
    [SerializeField] private Vector2 opponentHeroTransform;  // Герой супротивника (зверху)

    [SerializeField] public HeroesSO LocalHero;
    [SerializeField] public HeroesSO OpponentHero;
    private PhotonView photonView;
    private Transform enemyHero;
    private Transform owerHero;
   [SerializeField] private UnitsSOList owerUnits;
   [SerializeField] private UnitsSOList enemyUnits;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        photonView = GetComponent<PhotonView>();
        if (photonView == null)
        {
            photonView = gameObject.AddComponent<PhotonView>();
            photonView.ViewID = PhotonNetwork.AllocateViewID(PhotonNetwork.LocalPlayer.ActorNumber);
        }
    }

    private void Start()
    {
        // Ініціалізація після приєднання до кімнати
        StartCoroutine(InitializeAfterJoin());
    }

    private IEnumerator InitializeAfterJoin()
    {
        yield return new WaitUntil(() => PhotonNetwork.InRoom && PhotonNetwork.IsConnectedAndReady);
        Debug.Log($"Гравець {PhotonNetwork.NickName} приєднався до кімнати. IsMasterClient: {PhotonNetwork.IsMasterClient}");

        if (PhotonNetwork.IsMasterClient)
        {
            // Надсилання обом гравцям даних про героїв
            photonView.RPC(nameof(SyncHeroes), RpcTarget.All, LocalHero.name, OpponentHero.name);
        }
        else
        {
            // Non-Master чекає на синхронізацію
            Debug.Log("Non-Master чекає на дані про героїв...");
        }
    }

    [PunRPC]
    private void SyncHeroes(string localHeroName, string opponentHeroName)
    {

        LocalHero = Resources.Load<HeroesSO>("ScriptableObject/HeroesSO/" + localHeroName);
        OpponentHero = Resources.Load<HeroesSO>("ScriptableObject/HeroesSO/" + opponentHeroName);
        if (!PhotonNetwork.IsMasterClient)
        {
            // Ініціалізація свого героя
            owerHero = InitializeHero(heroTransform, LocalHero, true);
            HandManager.Instance.unitSOList = owerUnits;
            Debug.Log($"{PhotonNetwork.NickName} ініціалізував LocalHero: {LocalHero.name}");

            // Ініціалізація опонента
            enemyHero = InitializeHero(opponentHeroTransform, OpponentHero, false);
            Debug.Log($"{PhotonNetwork.NickName} ініціалізував OpponentHero: {OpponentHero.name}");
        }
        else
        {
            // Ініціалізація свого героя
            owerHero = InitializeHero(heroTransform, OpponentHero, true);
            Debug.Log($"{PhotonNetwork.NickName} ініціалізував LocalHero: {OpponentHero.name}");
            HandManager.Instance.unitSOList = enemyUnits;

            // Ініціалізація опонента
            enemyHero = InitializeHero(opponentHeroTransform, LocalHero, false);
            Debug.Log($"{PhotonNetwork.NickName} ініціалізував OpponentHero: {LocalHero.name}");
        }
    }

    private Transform InitializeHero(Vector2 target, HeroesSO data, bool isLocal)
    {

        GameObject hero = PhotonNetwork.Instantiate("Prefabs/Heroes/"+data.HeroPref.name, Vector3.zero, Quaternion.identity);
        hero.transform.SetParent(fatherForHeroes, false);

        hero.transform.SetSiblingIndex(0); // ставимо на найнижчу позицію в ієрархії
        hero.transform.localPosition = target;

        return hero.transform;
    }

    public Transform GetEnemyHeroes() => enemyHero;
    public Transform GetOwnerHeroes() => owerHero;
}