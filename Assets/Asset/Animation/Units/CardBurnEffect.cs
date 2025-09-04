using DG.Tweening;
using UnityEngine;
using System.Threading.Tasks;

public class CardBurnEffect : MonoBehaviour
{
    [SerializeField] private Material runtimeMaterial;
    [SerializeField] private float burnDuration = 2f;
    [SerializeField] private Material burnMaterialTemplate; // шаблон

    private void Awake()
    {
        runtimeMaterial = new Material(burnMaterialTemplate); // новий інстанс
                                                              // Прив'язуємо до спрайта або Image
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.material = runtimeMaterial;

        var img = GetComponent<UnityEngine.UI.Image>();
        if (img != null) img.material = runtimeMaterial;
    }

    public async Task StartBurnAsync()
    {
        runtimeMaterial.SetFloat("_BurnAmount", 0f);

        Tween burnTween = DOTween.To(
            () => runtimeMaterial.GetFloat("_BurnAmount"),
            x => runtimeMaterial.SetFloat("_BurnAmount", x),
            1f,
            burnDuration
        );

        await burnTween.AsyncWaitForCompletion(); // чекаємо завершення

/*        Destroy(gameObject, 0.1f);
*/    }
}
