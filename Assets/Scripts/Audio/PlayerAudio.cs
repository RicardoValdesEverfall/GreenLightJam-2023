using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [Header("FMOD")]
    [SerializeField] public FMODUnity.EventReference catMeowEvent;
    FMOD.Studio.EventInstance catMeowInstance;
    [SerializeField] public FMODUnity.EventReference catJumpEvent;
    FMOD.Studio.EventInstance catJumpInstance;


    InputManager inputManager;
    PlayerLocomotion playerLocomotion;

    Rigidbody cachedRigidBody;


    // Start is called before the first frame update
    void Start()
    {
        inputManager = GetComponent<InputManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        cachedRigidBody = GetComponent<Rigidbody>();
    }


    public void PlayMeow()
    {
        FMOD.Studio.PLAYBACK_STATE playbackState;
        catMeowInstance.getPlaybackState(out playbackState);

        if (playbackState == FMOD.Studio.PLAYBACK_STATE.STOPPED && inputManager.meowInput == true)
        {
            catMeowInstance = FMODUnity.RuntimeManager.CreateInstance(catMeowEvent);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(catMeowInstance, GetComponent<Transform>(), cachedRigidBody);
            catMeowInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject, cachedRigidBody));
            catMeowInstance.start();
            catMeowInstance.release();
        }
    }

    public void PlayJump()
    {
        FMOD.Studio.PLAYBACK_STATE playbackState;
        catJumpInstance.getPlaybackState(out playbackState);

        if (playbackState == FMOD.Studio.PLAYBACK_STATE.STOPPED/* && playerLocomotion.isJumping == true || playerLocomotion.isJumpingFromClimb == true */)
        {
            catJumpInstance = FMODUnity.RuntimeManager.CreateInstance(catJumpEvent);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(catJumpInstance, GetComponent<Transform>(), cachedRigidBody);
            catJumpInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject, cachedRigidBody));
            catJumpInstance.start();
            catJumpInstance.release();
        }
    }
}
