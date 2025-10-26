using Photon.Pun;
using UnityEngine;
using static TurnManager;

public class SpawnManager : MonoBehaviourPun
{
    [SerializeField] private Transform enemyHand, ownerHand;
    [SerializeField] private Transform card;
    public UnitsSOList unitSOList;
    public UnitsSOList enemySOList;

    public static SpawnManager Instance;
    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;
    }
    private void Start()
    {
        TurnManager.Instance.TurnChanged += On_TurnChanged;

    }
    private void On_TurnChanged(object sender, TurnManager.OnStateChangedEventArgs e)
    {
        if (e.state == TurnState.YourSpawning)
        {
            DrawCard();
        }
    }

    public void DrawCard()
    {

        int index = UnityEngine.Random.Range(0, unitSOList.UnitsSoList.Count);
        SpawnCard(index, false, -1);

    }
    [PunRPC()]
    public void SpawnCard(int unitSOIndex, bool noticed, int objectId)
    {
        GameObject cardObject;
        if (objectId == -1)
            cardObject = PhotonNetwork.Instantiate("Prefabs/" + card.name, Vector3.zero, Quaternion.identity);
        else
        {
            PhotonView pv = PhotonView.Find(objectId);
            cardObject = pv.gameObject;
        }

        CardInHand cardInHand = cardObject.GetComponent<CardInHand>();
        if (noticed)
        {
            cardObject.transform.SetParent(enemyHand);
           
            cardObject.transform.localPosition = Vector3.zero;
            cardInHand.IsEnemyCard = true;
            cardInHand.Initialize();

            enemyHand.GetComponent<OponentHandPanel>().AlignChildren();

        }
        else
        {
            photonView.RPC(nameof(SpawnCard), RpcTarget.Others, 0, true, cardObject.GetComponent<PhotonView>().ViewID);
            cardObject.transform.SetParent(ownerHand);
            UnitSO unitSO = unitSOList.UnitsSoList[unitSOIndex];
            
            cardObject.transform.localPosition = Vector3.zero;

            cardInHand.cardData = unitSO;
            cardInHand.Initialize();
            ownerHand.GetComponent<HandManager>().UpdateHand();


        }

    }
}
