using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FIMSpace.FSpine;

public class PlayerFootsteps : MonoBehaviour
{
    [Header("FMOD")]
    [SerializeField] public FMODUnity.EventReference footstepEvent;
    private FMOD.Studio.EventInstance Player_Footsteps;


    [SerializeField]
    private CURRENT_MATERIAL currentMaterial;

    private enum CURRENT_MATERIAL { Wood, Concrete, Grass, LaminatedWood, Plastic, ThinMetal };

    private Vector3 rayCastOffSet = new Vector3(0f, 0.1f, 0f);

    private void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        DetermineMaterial();

    }

    private void FixedUpdate()
    {
        RaycastFootHitDetection();
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
                PlayFootstep(0);
                break;


            case CURRENT_MATERIAL.Concrete:
                PlayFootstep(1);
                break;

            case CURRENT_MATERIAL.Grass:
                PlayFootstep(2);
                break;

            case CURRENT_MATERIAL.LaminatedWood:
                PlayFootstep(3);
                break;

            case CURRENT_MATERIAL.Plastic:
                PlayFootstep(4);
                break;

            case CURRENT_MATERIAL.ThinMetal:
                PlayFootstep(5);
                break;

            default:
                PlayFootstep(0);
                break;

        }
    }

    // Play FMOD footsteps
    public void PlayFootstep(int material)
    {
        Player_Footsteps = FMODUnity.RuntimeManager.CreateInstance(footstepEvent);
        Player_Footsteps.setParameterByName("Material", material);
        Player_Footsteps.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        Player_Footsteps.start();
        Player_Footsteps.release();
    }

    public void StopFootstep()
    {
        Player_Footsteps.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    public float raycastDistance = 0.03f;
    public Transform raycastOrigin;
    public Vector3 raycastOffset;
    Vector3 originPosition;
    Vector3 raycastDirection;
    Vector3 rotatedOffset;
    Vector3 raycastRotation;
    Vector3 rotationOffset;
    Quaternion rotation;
    private bool hasHitObject = false;
    public LayerMask layerToIgnore;
    public float hitBufferTime = 0.1f;
    private float lastHitTime = 0f;

    private void RaycastFootHitDetection()
    {
        RaycastHit hit;



        originPosition = raycastOrigin.position + raycastOrigin.TransformDirection(raycastOffset);
        raycastDirection = raycastOrigin.TransformDirection(Vector3.forward);
        


        bool isRaycastHit = Physics.Raycast(originPosition, raycastDirection, out hit, raycastDistance, ~layerToIgnore);

        if (isRaycastHit && !hasHitObject && Time.time - lastHitTime >= hitBufferTime)
        {
            SelectAndPlayFootstep();
            hasHitObject = true;
            lastHitTime = Time.time;
        }
        else if (!isRaycastHit)
        {
            hasHitObject = false;
        }

        Debug.DrawRay(originPosition, raycastDirection * raycastDistance, Color.red);

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(originPosition, raycastDirection * raycastDistance);
    }

}

/*
rotationOffset = rotation * raycastOffset;
originPosition = raycastOrigin.position + rotatedOffset;
raycastDirection = raycastOrigin.TransformDirection(Vector3.forward);
rotation = Quaternion.Euler(raycastRotation);
*/
