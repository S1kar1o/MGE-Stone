using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class CongratulationMenu : MonoBehaviour
{
    [SerializeField] private RawImage rawImage;
    [SerializeField] private VideoClip MGE;
    [SerializeField] private VideoClip Furri;
    [SerializeField] private Animator animator;
    private const string MGE_WON_TRIGER = "TesakDance";
    private const string FURRI_WON_TRIGER = "FurriWon";
    public static CongratulationMenu Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // якщо вже є інший екземпляр, видаляємо цей
            return;
        }
        Instance = this;
    }
    public void EndGamePanel(HeroOnScene hero)
    {
        VideoPlayer videoPlayer = rawImage.GetComponent<VideoPlayer>();
        if (hero.fraction == Fraction.MGE) {
            videoPlayer.clip = Furri;
            animator.SetTrigger(FURRI_WON_TRIGER);
        }
        else
        {
            videoPlayer.clip = MGE;
            animator.SetTrigger(MGE_WON_TRIGER);

        }
    }
}
