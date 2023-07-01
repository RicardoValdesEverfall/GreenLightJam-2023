using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMODUI : MonoBehaviour
{
    [SerializeField]
    private FMODUnity.EventReference uiHoverEvent;
    FMOD.Studio.EventInstance hoverInstance;

    [SerializeField]
    private FMODUnity.EventReference uiSelectEvent;
    FMOD.Studio.EventInstance selectInstance;

    public void PlayHover()
    {
        hoverInstance = FMODUnity.RuntimeManager.CreateInstance(uiHoverEvent);
        hoverInstance.start();
        hoverInstance.release();
    }

    public void PlaySelect()
    {
        selectInstance = FMODUnity.RuntimeManager.CreateInstance(uiSelectEvent);
        selectInstance.start();
        selectInstance.release();
    }
}
