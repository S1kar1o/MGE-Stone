using DG.Tweening;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitOnScene : MonoBehaviour
{
    public UnitSO cardData;
    [SerializeField] private AttackActionSO attackAction;
    [SerializeField] private AbilitySO abilitySo;
    public Image cardImage;
    public TextMeshProUGUI Damage;
    public TextMeshProUGUI Hp;
    public Fraction fraction;

    [SerializeField] private Animator animatorController;
    private const string ATTACK_TRIGER_ANIMATION = "Attack";
    private const string HEALING_TRIGER_ANIMATION = "GetHealing";
    private const string GET_DAMAGE_TRIGER = "GetDamage";
    private const string DEATH_TRIGER = "Death";
    [SerializeField] private CardBurnEffect cardBurnEffectScript;

    public int maxHp;
    public async Task Initialize()
    {
        if (cardData != null)
        {
            cardImage.sprite = cardData.cardSprite;
            Damage.text = cardData.Damage.ToString();
            Hp.text = cardData.Hp.ToString();
            fraction = cardData.Fraction;
            attackAction = cardData.attackAction;
            abilitySo = cardData.abilityAction;
            maxHp = int.Parse(Hp.text);
        }
        if (abilitySo != null)
        {
            if (abilitySo.needToIdentify)
            {
                AbilitySO attackActionOnStartSO = Instantiate(abilitySo);
                abilitySo = attackActionOnStartSO;
            }
            if (abilitySo.executeOnStart)
            {
                await abilitySo.Execute(gameObject);
            }
        }
    }
    public void boostDamage(int damageBoost) {
        Damage.text = (int.Parse(Damage.text)+ damageBoost).ToString();
        //start anim Boost Damage
    }
    public async Task GetDamage(int damage)
    {
        if (damage >= int.Parse(Hp.text))
        {
          
            await DestroyCard();
        }
        else
        {
            Hp.text = (int.Parse(Hp.text) - damage).ToString();
            await PlayTriggerAnimation(GET_DAMAGE_TRIGER);
        }
    }
    public async Task ChangeUnitTo(UnitSO newUnitData)
    {
        cardData = newUnitData;
        await Initialize();
    }
    public async Task DestroyCard()
    {
        Hp.text = 0.ToString();
        await Task.WhenAll(PlayTriggerAnimation(DEATH_TRIGER),
        cardBurnEffectScript.StartBurnPartialAsync()
           );

        Transform lineTransform = gameObject.transform.parent;
        LineScript lineScript = lineTransform.GetComponent<LineScript>();
        if (fraction == GameManager.Instance.GetOwnerHeroes().GetComponent<HeroOnScene>().fraction)
            lineScript.RemoveHero(gameObject.transform);
        transform.SetParent(lineTransform.transform.parent);
        await lineScript.UpdateHeroPositions();
        Destroy(gameObject);
    }
    public async Task GiveHealth(Transform target)
    {
        if (animatorController == null)
            animatorController = GetComponent<Animator>();

        Vector3 startPos = transform.position;
        Vector3 targetPos = target.position;
        var moveAnimTask = MoveWithDOTween(startPos, targetPos, target);
        var triggerAnimTask = PlayTriggerAnimation(HEALING_TRIGER_ANIMATION);

        target.GetComponent<UnitOnScene>().GetHealth(int.Parse(Damage.text));
        await Task.WhenAll(triggerAnimTask, moveAnimTask);
    }
    public void GetHealth(int healAmount)
    {
        if (int.Parse(Hp.text) > 0)
        {
            Hp.text = (int.Parse(Hp.text) + healAmount).ToString();
        }
    }
    public async Task StartSkill()
    {
        if (int.Parse(Hp.text) <= 0) return; 
        if (abilitySo != null)
        {
            if (abilitySo.multipleExecuted)
            {
                await abilitySo.Execute(gameObject);
            }
        }
    }
    public async Task Attack()
    {
        if (int.Parse(Hp.text) <= 0) return; 
        if (abilitySo != null)
        {
            if (abilitySo.attackSkill)
            {
                await abilitySo.Execute(gameObject);
            }
        }
        if (attackAction != null)
        {
            await attackAction.Execute(gameObject);
        }
        
    }
    public async Task PlayAttackAnimationWithMove(Transform target)
    {
        if (animatorController == null)
            animatorController = GetComponent<Animator>();

        Vector3 startPos = transform.position;
        Vector3 targetPos = target.position;

        var triggerAnimTask = PlayTriggerAnimation(ATTACK_TRIGER_ANIMATION);

        var moveAnimTask = MoveWithDOTween(startPos, targetPos, target);

        await Task.WhenAll(triggerAnimTask, moveAnimTask);


    }

    private async Task PlayTriggerAnimation(string nameOfTrigger)
    {
        animatorController.SetTrigger(nameOfTrigger);

        int attackHash = Animator.StringToHash("Base Layer." + nameOfTrigger);

        while (animatorController.GetCurrentAnimatorStateInfo(0).fullPathHash != attackHash)
            await Task.Yield();

        while (animatorController.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f &&
               animatorController.GetCurrentAnimatorStateInfo(0).fullPathHash == attackHash)
        {
            await Task.Yield();
        }
    }


    private async Task MoveWithDOTween(Vector3 startPos, Vector3 targetPos, Transform target)
    {
        float duration = 0.25f; // швидкість підльоту
        float pause = 0.1f;     // затримка на ударі

        Vector3 vector3 = new Vector3(startPos.x, targetPos.y, startPos.z);
        await transform.DOMove(vector3, duration).SetEase(Ease.OutQuad).AsyncWaitForCompletion();

        UnitOnScene enemy = target.GetComponent<UnitOnScene>();
        Task animationOfGetDamageByEnemy = null;

        if (enemy != null)
        {
            animationOfGetDamageByEnemy = enemy.GetDamage(int.Parse(Damage.text));
        }


        await Task.Delay((int)(pause * 1000));
        await transform.DOMove(startPos, duration).SetEase(Ease.InQuad).AsyncWaitForCompletion();

        if (animationOfGetDamageByEnemy != null)
        {
            await animationOfGetDamageByEnemy;
        }
    }
}
