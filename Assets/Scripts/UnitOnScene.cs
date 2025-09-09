using DG.Tweening;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitOnScene : MonoBehaviour
{
    public UnitSO cardData; // посилання на ScriptableObject
    [SerializeField] private AttackActionSO attackAction;
    [SerializeField] private AttackActionSO actionOnStart;
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
    public void Initialize()
    {
        if (cardData != null)
        {
            cardImage.sprite = cardData.cardSprite;
            Damage.text = cardData.Damage.ToString();
            Hp.text = cardData.Hp.ToString();
            fraction = cardData.Fraction;
            attackAction = cardData.attackAction;
            actionOnStart = cardData.actionOnStart;
        }
        if (actionOnStart != null)
        {
            actionOnStart.Execute(gameObject);
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
            Hp.text = 0.ToString();
            // Запускаємо обидві анімації паралельно
            await Task.WhenAll(
                PlayTriggerAnimation(DEATH_TRIGER),
                cardBurnEffectScript.StartBurnAsync()
            );
            await DestroyCard();
        }
        else
        {
            Hp.text = (int.Parse(Hp.text) - damage).ToString();
            await PlayTriggerAnimation(GET_DAMAGE_TRIGER);
        }

    }
    public async Task DestroyCard()
    {
        Transform lineTransform = gameObject.transform.parent;
        LineScript lineScript = lineTransform.GetComponent<LineScript>();
        if (fraction == GameManager.Instance.GetOwerHeroes().GetComponent<HeroOnScene>().fraction)
            lineScript.RemoveHero(gameObject.transform);
        transform.SetParent(null);
        await lineScript.UpdateHeroPositions();
        Destroy(gameObject);
    }
    public async Task GiveHealth(Transform target)
    {
        if (animatorController == null)
            animatorController = GetComponent<Animator>();

        Vector3 startPos = transform.position;
        Vector3 targetPos = target.position;

        // Запускаємо тригерну анімацію
        // Запускаємо рух вперед-назад
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
    public async Task Attack()
    {
        if (int.Parse(Hp.text) <= 0) return; // мертвий — не атакує
        await attackAction.Execute(gameObject);
    }
    public async Task PlayAttackAnimationWithMove(Transform target)
    {
        if (animatorController == null)
            animatorController = GetComponent<Animator>();

        Vector3 startPos = transform.position;
        Vector3 targetPos = target.position;

        // Запускаємо тригерну анімацію
        var triggerAnimTask = PlayTriggerAnimation(ATTACK_TRIGER_ANIMATION);

        // Запускаємо рух вперед-назад
        var moveAnimTask = MoveWithDOTween(startPos, targetPos, target);

        await Task.WhenAll(triggerAnimTask, moveAnimTask);


        Debug.Log(name + ": Attack animation (trigger + move) finished");
    }

    private async Task PlayTriggerAnimation(string nameOfTrigger)
    {
        animatorController.SetTrigger(nameOfTrigger);

        // Отримуємо hash потрібного стейта
        int attackHash = Animator.StringToHash("Base Layer." + nameOfTrigger);

        Debug.Log($"{nameOfTrigger}: {attackHash}");
        // Чекаємо поки ми ввійдемо у цей стейт
        while (animatorController.GetCurrentAnimatorStateInfo(0).fullPathHash != attackHash)
            await Task.Yield();

        // Чекаємо завершення анімації
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
        // Tween вперед
        await transform.DOMove(vector3, duration).SetEase(Ease.OutQuad).AsyncWaitForCompletion();

        UnitOnScene enemy = target.GetComponent<UnitOnScene>();
        Task animationOfGetDamageByEnemy = null;

        if (enemy != null)
        {
            animationOfGetDamageByEnemy = enemy.GetDamage(int.Parse(Damage.text));
        }


        await Task.Delay((int)(pause * 1000));
        //  пауза
        // Tween назад
        await transform.DOMove(startPos, duration).SetEase(Ease.InQuad).AsyncWaitForCompletion();

        if (animationOfGetDamageByEnemy != null)
        {
            await animationOfGetDamageByEnemy;
        }
    }
}
