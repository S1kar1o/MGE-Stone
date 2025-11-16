using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuSceneUIController : MonoBehaviour
{
    [SerializeField] private Button deckButton;
    [SerializeField] private Button startBattleButton;
    [SerializeField] private string deckSceneName;
    private void Start()
    {
        deckButton.onClick.AddListener(() =>
        {
            SceneLoader.Instance.LoadScene(deckSceneName);
        }); 
        startBattleButton.onClick.AddListener(() =>
        {
            PhotonConector.instance.StartBatle();
        });
    }
}
