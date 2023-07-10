using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    [Header("FMOD")]
    [SerializeField] public FMODUnity.EventReference musicInGame;
    [SerializeField] public FMODUnity.EventReference musicMenu;

    public string mainMenuScene, mainGameScene;
    Scene currentScene;


    FMODUnity.EventReference music;
    FMOD.Studio.EventInstance musicInstance;
    FMOD.Studio.EventInstance previousMusicInstance;






    private void PlayMusic(FMODUnity.EventReference music)
    {
        previousMusicInstance = musicInstance;
        musicInstance = FMODUnity.RuntimeManager.CreateInstance(music);
        musicInstance.start();
        //musicInstance.release();
    }

    private void Awake()
    {
        MusicManager[] musicManagers = FindObjectsOfType<MusicManager>();
        if (musicManagers.Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        SceneManager.activeSceneChanged += ChangedActiveScene;
    }

    private void Start()
    {
        /*
        currentScene = SceneManager.GetActiveScene();

     
        if (currentScene.name == mainGameScene)
        {
            music = musicInGame;
            PlayMusic(music);
        }
        else if (currentScene.name == mainMenuScene)
        {
            music = musicMenu;
            PlayMusic(music);
        }
        else
        {
            Debug.Log("No scene matches music event");

        }
        */
        
    }

    private void OnEnable()
    {
        SceneManager.activeSceneChanged += ChangedActiveScene;
    }

    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= ChangedActiveScene;
    }

    public void ChangedActiveScene(Scene previousScene, Scene changedScene)
    {
        if (previousMusicInstance.isValid() && previousMusicInstance.handle == musicInstance.handle)
        {
            previousMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            previousMusicInstance.release();
        }


        if (changedScene.name == mainMenuScene)
        {
            musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            musicInstance.release();
            PlayMusic(musicMenu);
        }
        else if (changedScene.name == mainGameScene)
        {
            musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            musicInstance.release();
            PlayMusic(musicInGame);
        }

    }

}
