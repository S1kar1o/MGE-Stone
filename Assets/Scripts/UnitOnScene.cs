using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitOnScene : MonoBehaviour
{
    public UnitSO cardData; // посилання на ScriptableObject
    [SerializeField] private AttackActionSO attackAction;
    public Image cardImage;
    public TextMeshProUGUI Damage;
    public TextMeshProUGUI Hp;
    public Fraction fraction;
    [SerializeField] public Animator animatorController;
    public void Initialize()
    {
        if (cardData != null)
        {
            cardImage.sprite = cardData.cardSprite;
            Damage.text = cardData.Damage.ToString();
            Hp.text = cardData.Hp.ToString();
            fraction = cardData.Fraction;
            attackAction = cardData.attackAction;
        }
    }
    public async Task GetDamage(int damage)
    {
        if (damage >= int.Parse(Hp.text))
        {
            Hp.text = 0.ToString();
            DestroyCard();
        }
        else
        {
            Hp.text = (int.Parse(Hp.text) - damage).ToString();
        }

    }
    public void DestroyCard()
    {
        Transform lineTransform = gameObject.transform.parent;
        LineScript lineScript=lineTransform.GetComponent<LineScript>();
        if(fraction==GameManager.Instance.GetOwerHeroes().GetComponent<HeroOnScene>().fraction )
            lineScript.RemoveHero(gameObject.transform);
        lineScript.UpdateHeroPositions();
        Destroy(gameObject);
    }
    public async Task Attack()
    {
       await attackAction.Execute(gameObject);
    }
    public async Task PlayAttackAnimation()
    {
        if (animatorController == null)
            animatorController = GetComponent<Animator>();

        animatorController.SetTrigger("Attack");
        Debug.Log(name + ": Attack Trigger set");

        // Чекаємо, поки аніматор увійде у стан "Attack"
        while (!animatorController.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            await Task.Yield();

        // Чекаємо, поки анімація закінчиться
        while (animatorController.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            await Task.Yield();

        Debug.Log(name + ": Attack animation finished");
    }
}
