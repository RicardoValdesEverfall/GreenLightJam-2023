using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneManager : MonoBehaviour
{
    [Header("CUTSCENE MANAGER COMPONENTS")]
    [SerializeField, ReadOnly] private PlayableDirector cutsceneDirector;
    [SerializeField, ReadOnly] private PlayerManager playerManager;

    [Header("CUTSCENE MANAGER SETTINGS")]
    [SerializeField] private float playbackSpeed;

   // Start is called before the first frame update
   void Start()
    {
        cutsceneDirector = GetComponent<PlayableDirector>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerManager != null)
        {
            if (cutsceneDirector.state == PlayState.Paused && playerManager.isCinematicPlaying)
            {
                playerManager.isCinematicPlaying = false;
                this.gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerManager = other.GetComponent<PlayerManager>();
            playerManager.isCinematicPlaying = true;
          
            cutsceneDirector.Play();
            cutsceneDirector.playableGraph.GetRootPlayable(0).SetSpeed(playbackSpeed);
        }
    }
}
