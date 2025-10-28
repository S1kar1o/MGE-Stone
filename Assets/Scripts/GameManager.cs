using UnityEngine;
using Photon.Pun;
using System.Collections;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Transform fatherForHeroes;
    [SerializeField] private Vector2 heroTransform;          // Локальний герой (знизу)
    [SerializeField] private Vector2 opponentHeroTransform;  // Герой супротивника (зверху)

    [SerializeField] public HeroesSO LocalHero;
    [SerializeField] public HeroesSO OpponentHero;
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
            photonView.RPC(nameof(SpawnHeroes), RpcTarget.Others, LocalHero.name, OpponentHero.name, false);
            SpawnHeroes(LocalHero.name, OpponentHero.name, true);
        }
    }

    [PunRPC]
    private void SpawnHeroes(string localHeroName, string opponentHeroName,bool isOwner)
    {
        if (!isOwner)
        {
            LocalHero = Resources.Load<HeroesSO>("ScriptableObject/HeroesSO/" + localHeroName);
            OpponentHero = Resources.Load<HeroesSO>("ScriptableObject/HeroesSO/" + opponentHeroName);
        }
        else
        {
            OpponentHero = Resources.Load<HeroesSO>("ScriptableObject/HeroesSO/" + localHeroName);
            LocalHero = Resources.Load<HeroesSO>("ScriptableObject/HeroesSO/" + opponentHeroName);
        }
        if (!PhotonNetwork.IsMasterClient)
        {
            // Ініціалізація свого героя
            owerHero = InitializeHero(heroTransform, LocalHero);
            int owerHeroId = owerHero.GetComponent<PhotonView>().ViewID;
            SpawnManager.Instance.unitSOList = owerUnits;
            SpawnManager.Instance.enemySOList = enemyUnits;



            // Ініціалізація опонента
            enemyHero = InitializeHero(opponentHeroTransform, OpponentHero);
            int EnemyHeroId = enemyHero.GetComponent<PhotonView>().ViewID;

            photonView.RPC(nameof(SetPositionForOponent), RpcTarget.Others, EnemyHeroId, owerHeroId);
        }
        else
        {


            SpawnManager.Instance.unitSOList = enemyUnits;
            SpawnManager.Instance.enemySOList = owerUnits;


        }
    }

    private Transform InitializeHero(Vector2 target, HeroesSO data)
    {

        GameObject hero = PhotonNetwork.Instantiate("Prefabs/Heroes/" + data.HeroPref.name, Vector3.zero, Quaternion.identity);
        hero.transform.SetParent(fatherForHeroes, false);

        hero.transform.SetSiblingIndex(0); // ставимо на найнижчу позицію в ієрархії
        hero.transform.localPosition = target;

        return hero.transform;
    }
    [PunRPC]
    private void SetPositionForOponent(int oponentHeroId, int owerHeroId)
    {
        PhotonView pv = PhotonView.Find(oponentHeroId);
        GameObject oponentHeroGameObject = pv.gameObject;

        oponentHeroGameObject.transform.SetParent(fatherForHeroes, false);

        oponentHeroGameObject.transform.SetSiblingIndex(0); // ставимо на найнижчу позицію в ієрархії
        oponentHeroGameObject.transform.localPosition = heroTransform;
        owerHero=oponentHeroGameObject.transform;
        PhotonView pvOwer = PhotonView.Find(owerHeroId);
        GameObject owerHeroGameObject = pvOwer.gameObject;

        owerHeroGameObject.transform.SetParent(fatherForHeroes, false);

        owerHeroGameObject.transform.SetSiblingIndex(0); // ставимо на найнижчу позицію в ієрархії
        owerHeroGameObject.transform.localPosition = opponentHeroTransform;
        enemyHero = owerHeroGameObject.transform;

    }

    public Transform GetEnemyHeroes() => enemyHero;
    public Transform GetOwnerHeroes() => owerHero;
}