using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [Header("FMOD")]
    [SerializeField] public FMODUnity.EventReference catMeowEvent;
    FMOD.Studio.EventInstance catMeowInstance;
    private FMOD.Studio.EventInstance Player_Footsteps;

    InputManager inputManager;


    [SerializeField]
    private CURRENT_MATERIAL currentMaterial;

    private enum CURRENT_MATERIAL { Wood, Concrete, Grass, LaminatedWood, Plastic, ThinMetal};

    private Vector3 rayCastOffSet = new Vector3(0f, 0.1f, 0f);


    Rigidbody cachedRigidBody;

    // Start is called before the first frame update
    void Start()
    {
        inputManager = GetComponent<InputManager>();
        cachedRigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        DetermineMaterial();    
        
    }


    // Raycase below to see what material layer it is
    private void DetermineMaterial()
    {

        RaycastHit hit;
        Ray ray = new Ray(transform.position + rayCastOffSet, Vector3.down);
        //Debug.DrawRay(transform.position + rayCastOffSet, Vector3.down, Color.red);
       


        if (Physics.Raycast(ray, out hit, 1.0f, Physics.AllLayers, QueryTriggerInteraction.Ignore))
        {

            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Wood"))
            {
                currentMaterial = CURRENT_MATERIAL.Wood;
            }

            else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Concrete"))
            {
                currentMaterial = CURRENT_MATERIAL.Concrete;
            }

            else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Grass"))
            {
                currentMaterial = CURRENT_MATERIAL.Grass;
            }

            else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("LaminatedWood"))
            {
                currentMaterial = CURRENT_MATERIAL.LaminatedWood;
            }

            else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Plastic"))
            {
                currentMaterial = CURRENT_MATERIAL.Plastic;
            }

            else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("ThinMetal"))
            {
                currentMaterial = CURRENT_MATERIAL.ThinMetal;
            }

            //Debug.Log(currentMaterial);
        }
    }


    // Method to switch materials in FMOD
    public void SelectAndPlayFootstep()
    {
        switch (currentMaterial)

        {
            case CURRENT_MATERIAL.Wood:
                PlayFootsteps(0);
                break;


            case CURRENT_MATERIAL.Concrete:
                PlayFootsteps(1);
                break;

            case CURRENT_MATERIAL.Grass:
                PlayFootsteps(2);
                break;

            case CURRENT_MATERIAL.LaminatedWood:
                PlayFootsteps(3);
                break;

            case CURRENT_MATERIAL.Plastic:
                PlayFootsteps(4);
                break;

            case CURRENT_MATERIAL.ThinMetal:
                PlayFootsteps(5);
                break;

            default:
                PlayFootsteps(0);
                break;

        }
    }

    // Play FMOD footsteps
    public void PlayFootsteps(int material)
    {
        Player_Footsteps = FMODUnity.RuntimeManager.CreateInstance("event:/Player_Footsteps");
        Player_Footsteps.setParameterByName("Material", material);
        Player_Footsteps.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        Player_Footsteps.start();
        Player_Footsteps.release();
    }

    public void StopFootsteps()
    {
        Player_Footsteps.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
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
            Debug.Log("Cat meow played");
            catMeowInstance.release();
        }
    }


}
