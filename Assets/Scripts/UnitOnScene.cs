using DG.Tweening;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitOnScene : MonoBehaviour
{
    public UnitSO cardData; // ��������� �� ScriptableObject
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
    public async Task PlayAttackAnimationWithMove(Transform target)
    {
        if (animatorController == null)
            animatorController = GetComponent<Animator>();

        Vector3 startPos = transform.position;
        Vector3 targetPos = target.position;

        // ��������� �������� �������
        var triggerAnimTask = PlayTriggerAnimation();

        // ��������� ��� ������-�����
        var moveAnimTask = MoveWithDOTween(startPos, targetPos);
       /* Task animationOfGetDamageHero = null;

        if (heroe != null)
        {
            animationOfGetDamageHero = heroe.GetDamage(int.Parse(Damage.text));
        }

        // ������ �� �������, ����� ���� ���� �� null
        var tasks = new List<Task> { triggerAnimTask, moveAnimTask };
        if (animationOfGetDamageHero != null)
            tasks.Add(animationOfGetDamageHero);*/

        await Task.WhenAll(triggerAnimTask, moveAnimTask);

        Debug.Log(name + ": Attack animation (trigger + move) finished");
    }

    private async Task PlayTriggerAnimation()
    {
        animatorController.SetTrigger("Attack");

        // �������� hash ��������� ������
        int attackHash = Animator.StringToHash("Base Layer.Attack");

        // ������ ���� �� ������� � ��� �����
        while (animatorController.GetCurrentAnimatorStateInfo(0).fullPathHash != attackHash)
            await Task.Yield();

        // ������ ���������� �������
        while (animatorController.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f &&
               animatorController.GetCurrentAnimatorStateInfo(0).fullPathHash == attackHash)
        {
            await Task.Yield();
        }
    }


    private async Task MoveWithDOTween(Vector3 startPos, Vector3 targetPos)
    {
        float duration = 0.25f; // �������� �������
        float pause = 0.1f;     // �������� �� ����

        Vector3 vector3 = new Vector3(startPos.x, targetPos.y, targetPos.z);
        // Tween ������
        await transform.DOMove(vector3, duration).SetEase(Ease.OutQuad).AsyncWaitForCompletion();

        await Task.Delay((int)(pause * 1000)); // �������� �����
        // Tween �����
        await transform.DOMove(startPos, duration).SetEase(Ease.InQuad).AsyncWaitForCompletion();
        Debug.Log(12121);
    }
}
