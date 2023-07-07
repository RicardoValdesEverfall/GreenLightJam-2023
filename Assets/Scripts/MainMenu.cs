using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.VFX;

public class MainMenu : MonoBehaviour
{
    [Header("Loading screen")]
    [SerializeField] CanvasGroup loadingBackground;
    [SerializeField] CanvasGroup loadingItems;
    [SerializeField] string gameScene;
    

    [Header("UI")]
    [SerializeField] CanvasGroup windowMain;
    [SerializeField] CanvasGroup windowCredits;
    [SerializeField] CanvasGroup logo;
    [SerializeField] VisualEffect activeEffect;

    private CanvasGroup currentWindow;

    // Start is called before the first frame update
    void Start()
    {
        IntroMainMenu();
    }

    private void Awake()
    {
        loadingBackground.alpha = 0;
        loadingBackground.gameObject.SetActive(false);
        windowMain.alpha = 0;
        windowCredits.gameObject.SetActive(false);
        windowCredits.alpha = 0;
        logo.alpha = 0;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void IntroMainMenu()
    {
        var seq = DOTween.Sequence();
        seq.SetEase(Ease.InQuad);
        seq.SetDelay(3);
        seq.Append(logo.DOFade(1, 4));
        seq.Append(windowMain.DOFade(1, .5f));
        seq.OnComplete(() =>
        {
            currentWindow = windowMain;
        });
    }

    public void StartGame()
    {
        activeEffect.Stop();
        loadingBackground.gameObject.SetActive(true);
        var seq = DOTween.Sequence();
        //lets get the position
        seq.Append(loadingBackground.DOFade(1, 0.5f));
        seq.Append(loadingItems.DOFade(0, 1).SetDelay(10f));
        seq.OnComplete(() =>
        {
            SceneManager.LoadScene(gameScene);
        });

        //StartCoroutine(LoadingScreen());
    }

    public void GoBack()
    {
        var seq = DOTween.Sequence();
        //lets get the position
        seq.Append(currentWindow.DOFade(0, 1).OnComplete(() => {
            currentWindow.gameObject.SetActive(false);
            windowMain.gameObject.SetActive(true);
        }));
        seq.Append(windowMain.DOFade(1, 1).SetDelay(.5f));
        seq.OnComplete(() =>
        {
            currentWindow = windowMain;
        });
    }

    public void ShowCredits()
    {
        var seq = DOTween.Sequence();
        //lets get the position
        seq.Append(currentWindow.DOFade(0, 1).OnComplete(() => {
            currentWindow.gameObject.SetActive(false);
            windowCredits.gameObject.SetActive(true);
        }));
        seq.Append(windowCredits.DOFade(1, 1).SetDelay(.5f));
        seq.OnComplete(() =>
        {
            currentWindow = windowCredits;
        });
    }

    IEnumerator LoadingScreen()
    {
        loadingBackground.gameObject.SetActive(true);
        var seq = DOTween.Sequence();
        //lets get the position

        loadingBackground.DOFade(1, 1);
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene(gameScene);
    }
}
