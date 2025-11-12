using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class CardInHand : MonoBehaviourPunCallbacks, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private LayerMask lineLayer;

    private Vector3 originalPosition;
    private Vector3 originalScale;
    private int originalSiblingIndex;

    public float hoverHeight = 40f;
    public float scaleUp = 1.2f;
    public float moveDuration = 0.2f;

    private Sequence shakeSequence;
    private static CardInHand selectedCard;
    private bool isSelected = false;

    [SerializeField] public bool IsEnemyCard = false;
    public bool IsSelected
    {
        get { return isSelected; }
        set { isSelected = value; }
    }
    public UnitSO cardData; // посилання на ScriptableObject
    public Image cardImage;
    public TextMeshProUGUI Damage;
    public TextMeshProUGUI Hp;
    public TextMeshProUGUI costText;
    [SerializeField] private HeroOnScene ownerHero;
    [SerializeField] private Animator animator;

    [SerializeField] private Transform enemyVisual, ownerVisual;
    private void Start()
    {
        ownerHero = GameManager.Instance.GetOwnerHeroes().GetComponent<HeroOnScene>();
        if (IsEnemyCard)
        {
            enemyVisual.gameObject.SetActive(true);
            //this.enabled = false;
        }
        else
        {
            ownerVisual.gameObject.SetActive(true);
        }
    }
    public void Initialize()
    {
        if (IsEnemyCard)
        {
            ownerHero = GameManager.Instance.GetEnemyHeroes().GetComponent<HeroOnScene>();

            enemyVisual.gameObject.SetActive(true);
            this.enabled = false;
        }
        else
        {
            ownerVisual.gameObject.SetActive(true);
        }
        if (cardData != null)
        {
            cardImage.sprite = cardData.cardSprite;
            Damage.text = cardData.Damage.ToString();
            Hp.text = cardData.Hp.ToString();
            costText.text = cardData.cost.ToString();
            originalPosition = transform.localPosition;
            originalScale = transform.localScale;
            originalSiblingIndex = transform.GetSiblingIndex();

            /*            animator.SetTrigger("CardSpawnedTrigger");
            */
        }
    }
    public void DOKillAll()
    {
        transform.DOKill(false);
    }
    private void Update()
    {
        if (isSelected && Input.GetMouseButtonDown(0))
        {
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero, Mathf.Infinity, lineLayer);

            if (hit.collider != null)
            {
                LineScript targetLine = hit.collider.GetComponent<LineScript>();
                if (targetLine != null)
                {
                    Debug.Log("Clicked line: " + targetLine.name);
                    int unitIndex = SpawnManager.Instance.unitSOList.UnitsSoList.IndexOf(cardData);
                    if (int.Parse(ownerHero.Mana.text) >= cardData.cost)
                    {
                        TryPlaceCard(targetLine.name, unitIndex, true);
                        photonView.RPC(nameof(TryPlaceCard), RpcTarget.Others, targetLine.name, unitIndex, false);
                    }
                    else
                    {
                        animator.SetTrigger("NotEnoughtManaTrigger");
                        Debug.Log("Failed to place card on line: " + targetLine.name);
                    }
                }
            }
            else
            {
                Debug.Log("No line found under mouse.");
            }
        }
    }
    [PunRPC]
    private void TryPlaceCard(string targetLineName, int unitIndexInList, bool isOwner)
    {
        GameObject targetLineGameObject = GameObject.Find(targetLineName);
        LineScript targetLine = targetLineGameObject.GetComponent<LineScript>();
        UnitSO unitSO;
        if (isOwner)
            unitSO = SpawnManager.Instance.unitSOList.UnitsSoList[unitIndexInList];
        else
            unitSO = SpawnManager.Instance.enemySOList.UnitsSoList[unitIndexInList];


        if (cardData == null)
            cardData = unitSO;


        bool placed = PlaceOnLine(targetLine, isOwner);
        if (placed)
        {
            selectedCard = null;
            if (isOwner)
            {
                ownerHero.Mana.text = (int.Parse(ownerHero.Mana.text) - unitSO.cost).ToString();
                HandManager.Instance.UpdateHand();
            }
            else
            {
                GameManager.Instance.GetEnemyHeroes().GetComponent<HeroOnScene>().Mana.text =
                    (int.Parse(GameManager.Instance.GetEnemyHeroes().GetComponent<HeroOnScene>().Mana.text) - unitSO.cost).ToString();
                OponentHandPanel.Instance.AlignChildren();
            }
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Not enough mana to place card.");
            animator.SetTrigger("NotEnoughtManaTrigger");
        }
    }
    public void RefreshOriginalPosition()
    {
        if (!DOTween.IsTweening(transform))
        {
            originalPosition = transform.localPosition;
            if (!isSelected)
                transform.localPosition = originalPosition;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isSelected) return;

        transform.DOKill();
        transform.DOLocalMoveY(originalPosition.y + hoverHeight, moveDuration);
        transform.DOScale(originalScale * scaleUp, moveDuration);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isSelected) return;

        transform.DOKill();
        transform.DOLocalMove(originalPosition, moveDuration);
        transform.DOScale(originalScale, moveDuration);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isSelected)
        {
            DeselectCard();
            selectedCard = null;
        }
        else
        {
            if (selectedCard != null && selectedCard != this)
            {
                selectedCard.DeselectCard();
            }
            SelectCard();
        }
    }

    public bool PlaceOnLine(LineScript line, bool isOwner)
    {
        if (isOwner)
            if (!isSelected) return false;

        DOKillAll();
        KillShake();
        bool canPlace;
        if (isOwner)
            canPlace = line.CheckPosibilityToAddACard();
        else
            canPlace = line.CheckPosibilityToAddEnemyCard();

        _ = line.TryAddCardOnLine(cardData, isOwner);
        if (canPlace)
        {
            isSelected = false;
            selectedCard = null;
            transform.localScale = originalScale;
            transform.localRotation = Quaternion.identity;

            HandManager handManager = GetComponentInParent<HandManager>();
            if (handManager != null)
            {
                handManager.UpdateHand();
            }
            else
            {
                OponentHandPanel oponentHandPanel = GetComponentInParent<OponentHandPanel>();
                oponentHandPanel.AlignChildren();
            }
        }

        return canPlace;
    }

    private void SelectCard()
    {
        isSelected = true;
        selectedCard = this;

        transform.DOKill();
        transform.SetAsLastSibling();

        transform.DOLocalMoveY(originalPosition.y + hoverHeight, moveDuration);
        transform.DOScale(originalScale * scaleUp, moveDuration);

        StartShake();
    }

    private void DeselectCard()
    {
        isSelected = false;
        KillShake();

        transform.DOKill();
        transform.SetSiblingIndex(originalSiblingIndex);
        transform.DOLocalMove(originalPosition, moveDuration);
        transform.DOScale(originalScale, moveDuration);
    }
    private void StartShake()
    {
        // shakeSequence керує CardContainer
        KillShake();

        shakeSequence = DOTween.Sequence();

        shakeSequence.Append(transform.DOLocalRotate(new Vector3(0, 0, 10f), 0.15f).SetEase(Ease.InOutSine));
        shakeSequence.Join(transform.DOShakePosition(0.3f, new Vector3(8f, 0, 0), 6));
        shakeSequence.Append(transform.DOLocalRotate(new Vector3(0, 0, -10f), 0.15f).SetEase(Ease.InOutSine));
        shakeSequence.Join(transform.DOShakePosition(0.3f, new Vector3(8f, 0, 0), 6));
        shakeSequence.Append(transform.DOLocalRotate(Vector3.zero, 0.15f).SetEase(Ease.InOutSine));

        shakeSequence.SetLoops(-1, LoopType.Restart).SetId("shake");
    }

    private void KillShake()
    {
        if (shakeSequence != null && shakeSequence.IsActive())
        {
            shakeSequence.Kill();
            shakeSequence = null;
        }
        transform.localRotation = Quaternion.identity;
    }

}