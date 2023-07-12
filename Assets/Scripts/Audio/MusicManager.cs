using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;
using System;
using UnityEngine.Events;

public class MusicManager : MonoBehaviour
{
    [Header("Scripts")]
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private PlayerLocomotion playerLocomotion;

    [Header("FMOD")]
    [SerializeField] public FMODUnity.EventReference musicInGame;
    [SerializeField] public FMODUnity.EventReference musicMenu;

    //FMOD Event Instances
    FMOD.Studio.EventInstance musicInstance;
    FMOD.Studio.EventInstance previousMusicInstance;

    //Scene Variables
    public string mainMenuScene, mainGameScene;
    Scene currentScene;

    //Item Variables
    private int itemCounter;

    [Header("FMOD Intensity Parameter")]
    [SerializeField] private float sprintingIntensity = 0.05f;
    [SerializeField] private float jumpingIntensity = 0.1f;
    [SerializeField] private float climbingIntensity = 0.2f;
    [SerializeField] private float fallingIntensity = 20f;
    [SerializeField] private float intensityDecreaseRate = 60f;
    [SerializeField] private float currentIntensityValue;



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


    private void OnEnable()
    {
        SceneManager.activeSceneChanged += ChangedActiveScene;
        //itemCounter += UpdateItemParameter(itemCounter);

    }


    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= ChangedActiveScene;
        musicInstance.release();
        //itemCounter -= UpdateItemParameter(itemCounter);
    }


    private void OnDestroy()
    {
        musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        musicInstance.release();
    }
    

    private void PlayMusic(FMODUnity.EventReference music)
    {
        previousMusicInstance = musicInstance;
        musicInstance = FMODUnity.RuntimeManager.CreateInstance(music);
        musicInstance.start();
        //musicInstance.release();
    }

    //Changes music depending on the current scene
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


    //Updates the music item parameter when you collect an item
    public void UpdateItemParameter(int itemCounter)
    {
        switch (itemCounter)
        {
            case 0:
                musicInstance.setParameterByName("Item", 0);
                break;

            case 1:
                musicInstance.setParameterByName("Item", 1);
                break;

            case 2:
                musicInstance.setParameterByName("Item", 2);
                break;

            case 3:
                musicInstance.setParameterByName("Item", 3);
                break;

            case 4:
                musicInstance.setParameterByName("Item", 4);
                break;

            case 5:
                musicInstance.setParameterByName("Item", 5);
                break;
        }
    }

    //Increases the music intensity parameter when sprinting, jumping, climbing, and falling
    private void MusicIntensity()
    {
        musicInstance.getParameterByName("Intensity", out currentIntensityValue);

        //Sprinting
        if (playerLocomotion.isSprinting)
        {
            currentIntensityValue += sprintingIntensity;
        }

        //Jumping
        if (playerLocomotion.isJumping || playerLocomotion.isJumpingFromClimb)
        {
            currentIntensityValue += jumpingIntensity;
        }

        //Climbing
        if (playerLocomotion.isClimbing)
        {
            currentIntensityValue += climbingIntensity;
        }

        //Falling
        if (playerLocomotion.isFalling)
        {
            currentIntensityValue += fallingIntensity;
        }


        musicInstance.setParameterByName("Intensity", currentIntensityValue);
    }

    //The intensity parameter lowers itself over time
    private void LowerIntensityOverTime()
    {
        musicInstance.getParameterByName("Intensity", out currentIntensityValue);
        currentIntensityValue -= intensityDecreaseRate * Time.deltaTime;
        musicInstance.setParameterByName("Intensity", currentIntensityValue);
    }

    private void Update()
    {
       MusicIntensity();
       LowerIntensityOverTime();
    }

}
