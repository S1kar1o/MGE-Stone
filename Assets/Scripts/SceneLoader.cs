using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private GameObject loaderCanvas;
    [SerializeField] private Image progresBar;
   public static SceneLoader Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }
    IEnumerator LoadSceneCoroutine(string sceneName)
    {
        var loadProgress= SceneManager.LoadSceneAsync(sceneName);
        loadProgress.allowSceneActivation = false;
        loaderCanvas.SetActive(true);
        while (loadProgress.progress<0.9f)
        {
            progresBar.fillAmount= loadProgress.progress;
            yield return null;
        }
        loadProgress.allowSceneActivation = true;
        loaderCanvas.SetActive(false);

    }

}
