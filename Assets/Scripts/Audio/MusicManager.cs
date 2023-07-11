using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;
using System;

public class MusicManager : MonoBehaviour
{
    [Header("Scripts")]
    [SerializeField] private PlayerManager playerManager;

    [Header("FMOD")]
    [SerializeField] public FMODUnity.EventReference musicInGame;
    [SerializeField] public FMODUnity.EventReference musicMenu;

    public string mainMenuScene, mainGameScene;
    Scene currentScene;


    FMODUnity.EventReference music;
    FMOD.Studio.EventInstance musicInstance;
    FMOD.Studio.EventInstance previousMusicInstance;


    [StructLayout(LayoutKind.Sequential)]
    class TimelineInfo
    {
        public int currentMusicBeat = 0;
        public FMOD.StringWrapper lastMarker = new FMOD.StringWrapper();
    }

    TimelineInfo timelineInfo;
    GCHandle timelineHandle;

    FMOD.Studio.EVENT_CALLBACK beatCallback;

    private void Start()
    {
        timelineInfo = new TimelineInfo();

        beatCallback = new FMOD.Studio.EVENT_CALLBACK(BeatEventCallback);

        //Pin the class that will store the data modified during the callback
        timelineHandle = GCHandle.Alloc(timelineInfo);
        //Pass the object through the userdata of the instance
        musicInstance.setUserData(GCHandle.ToIntPtr(timelineHandle));

        musicInstance.setCallback(beatCallback, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT | FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER);
               
    }

    private void OnGUI()
    {
        GUILayout.Box(string.Format("Current Bar = {0}, Last Marker = {1}", timelineInfo.currentMusicBeat, (string)timelineInfo.lastMarker));
    }

    private void OnDestroy()
    {
        musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        musicInstance.release();
    }

    [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
    static FMOD.RESULT BeatEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
    {
        FMOD.Studio.EventInstance instance = new FMOD.Studio.EventInstance(instancePtr);

        //Retrieve the user data
        IntPtr timelineInfoPtr;
        FMOD.RESULT result = instance.getUserData(out timelineInfoPtr);
        if (result != FMOD.RESULT.OK)
        {
            Debug.LogError("Timeline Callback error: " + result);
        }
        else if (timelineInfoPtr != IntPtr.Zero)
        {
            //Get the object to store beat and marker details
            GCHandle timelineHandle = GCHandle.FromIntPtr(timelineInfoPtr);
            TimelineInfo timelineInfo = (TimelineInfo)timelineHandle.Target;

            switch (type)
            {
                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT:
                    {
                        var parameter = (FMOD.Studio.TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_BEAT_PROPERTIES));
                        timelineInfo.currentMusicBeat = parameter.bar;
                        break;
                    }
                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER:
                    {
                        var parameter = (FMOD.Studio.TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_MARKER_PROPERTIES));
                        timelineInfo.lastMarker = parameter.name;
                        break;
                    }
                case FMOD.Studio.EVENT_CALLBACK_TYPE.DESTROYED:
                    {
                        //Now the event has been destroyed, unpin the timeline memory so it can be garbage collected
                        timelineHandle.Free();
                        break;
                    }
            }

        }
        return FMOD.RESULT.OK;

    }

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
