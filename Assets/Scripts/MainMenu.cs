using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.VFX;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Loading screen")]
    [SerializeField] CanvasGroup loadingBackground;
    [SerializeField] CanvasGroup loadingItems;
    [SerializeField] string gameScene;
    [SerializeField] string mainMenuScene;


    [Header("UI")]
    [SerializeField] CanvasGroup windowMain;
    [SerializeField] CanvasGroup windowCredits;
    [SerializeField] CanvasGroup logo;
    [SerializeField] VisualEffect activeEffect;
    [SerializeField] Button back;
    [SerializeField] Button play;

    private CanvasGroup currentWindow;

    //FMOD
    //List banks to load
    [FMODUnity.BankRef]
    public List<string> gameBanks = new List<string>();
    [FMODUnity.BankRef]
    public List<string> mainMenuBanks = new List<string>();


    // Start is called before the first frame update
    void Start()
    {
        IntroMainMenu();
    }

    private void Awake()
    {

        StartCoroutine(LoadBanks(mainMenuBanks));
        loadingBackground.alpha = 0;
        loadingBackground.gameObject.SetActive(false);
        loadingItems.alpha = 0;
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
            play.Select();
        });
    }

    public void StartGame()
    {
        StartCoroutine(LoadScene(gameScene, gameBanks));
        activeEffect.Stop();
        loadingBackground.gameObject.SetActive(true);

        var seq = DOTween.Sequence();

        //lets get the position
        seq.Append(loadingBackground.DOFade(1, 0.5f));
        seq.Append(loadingItems.DOFade(1, 1f));
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
            play.Select();
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
            back.Select();
        });

        
    }

    IEnumerator LoadBanks(List<string> bankFMOD)
    {
        foreach (var bank in bankFMOD)
        {
            FMODUnity.RuntimeManager.LoadBank(bank, true);
            Debug.Log("Foreach LoadBank");
        }

        while (!FMODUnity.RuntimeManager.HaveAllBanksLoaded)
        {
            Debug.Log("HaveAllBanksLoaded");
            yield return null;
        }

        while (FMODUnity.RuntimeManager.AnySampleDataLoading())
        {
            Debug.Log("AnySampleDataLoading");
            yield return null;
        }

    }


    IEnumerator LoadScene(string sceneToLoad, List<string> bankFMOD)
    {
        //loadingBackground.gameObject.SetActive(true);        

        Scene currentScene = SceneManager.GetActiveScene();

        AsyncOperation async = SceneManager.LoadSceneAsync(sceneToLoad);
        async.allowSceneActivation = false;

        StartCoroutine(LoadBanks(bankFMOD));
             
        while (!async.isDone)
        {
            float progressValue = Mathf.Clamp01(async.progress / 0.9f);
            //Debug.Log("Async progress: " + progressValue);
            yield return null;
        }                  
        
        //Finished loading
        var seq = DOTween.Sequence();
        seq.Append(loadingBackground.DOFade(1, 0.5f));
        seq.Append(loadingItems.DOFade(1, 1f));
        seq.Append(loadingItems.DOFade(0, 1));
        async.allowSceneActivation = true;
        //inputModule.enabled = true;
        Debug.Log("Scene: " + sceneToLoad + " has loaded");
    }


}
