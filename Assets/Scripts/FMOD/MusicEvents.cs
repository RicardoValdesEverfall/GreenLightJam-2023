using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
[CreateAssetMenu(menuName = "Scriptable Objects/Audio/Music")]
public class MusicEvents : ScriptableObject
{
    [Header("Music Events")]
    [FMODUnity.EventReference.Find(musicInGame)]
    public string musicInGame = null;

}







/*

[CreateAssetMenu(menuName = "Scriptable Objects/Audio/Music")]
public class MusicEvents : ScriptableObject
{
    [Header("Music Events")]
    public List<AssetReference> musicEvents = new List<AssetReference>();

    public List<FMOD.Studio.EventInstance> GetEventInstances()
    {
        List<FMOD.Studio.EventInstance> instances = new List<FMOD.Studio.EventInstance>();

        foreach (AssetReference assetRef in musicEvents)
        {
            AsyncOperationHandle<AudioClip> handle = assetRef.LoadAssetAsync<AudioClip>();
            handle.Completed += operation =>
            {

                if (operation.Status == AsyncOperationStatus.Succeeded)
                {
                    AudioClip audioClip = operation.Result;
                    FMODUnity.RuntimeManager.StudioSystem.getEvent(audioClip.name, out FMOD.Studio.EventDescription description);
                    description.createInstance(out FMOD.Studio.EventInstance instance);
                    instances.Add(instance);

                }
            };

        }

        return instances;
    }

}





/*
[System.Serializable]
public class EventReference
{
    public string musicInGame = null;
}
*/