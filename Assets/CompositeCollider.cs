using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompositeCollider : MonoBehaviour
{
    [Header("FMOD")]
    [SerializeField] public FMODUnity.EventReference reference;
    FMOD.Studio.EventInstance instance;
    int colliderCount = 0;

    public GameObject combinedColliders;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (colliderCount == 0)
            {
                Play(reference);
            }
            colliderCount++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            colliderCount--;
            if (colliderCount == 0)
            {
                Stop();
            }
        }
    }

    private void Play(FMODUnity.EventReference reference)
    {
        instance = FMODUnity.RuntimeManager.CreateInstance(reference);
        instance.start();
        instance.release();
    }

    private void Stop()
    {
        instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        instance.release();
    }
}
