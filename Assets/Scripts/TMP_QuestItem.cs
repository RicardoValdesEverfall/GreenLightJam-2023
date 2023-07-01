using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class TMP_QuestItem : MonoBehaviour
{
    [SerializeField] private GameObject assetToFocusOn;
    [SerializeField] private Camera cinematicCamera;
    [SerializeField] private Camera playerCam;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private AudioClip narrativeClip;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if (assetToFocusOn != null)
            {
                playerManager.isCinematicPlaying = true;
                var seq = DOTween.Sequence();
                //lets get the position
                Vector3 posAsset = assetToFocusOn.transform.position;
                Debug.Log("asset pos: " + posAsset);
                posAsset += (assetToFocusOn.transform.forward.normalized * 6);
                Debug.Log("final pos of cam: " + posAsset);
                seq.SetEase(Ease.InCubic);

                cinematicCamera.transform.position = playerCam.transform.position;
                cinematicCamera.transform.rotation = playerCam.transform.rotation;
                cinematicCamera.GetComponent<Camera>().fieldOfView = playerCam.fieldOfView;
                playerCam.gameObject.SetActive(false);
                cinematicCamera.gameObject.SetActive(true);

                seq.Append(cinematicCamera.transform.DOMove(posAsset, 8f).OnComplete(() => {
                    StartCoroutine(NarrativeItem());
                    
                }));
                seq.Join(cinematicCamera.transform.DOLookAt(assetToFocusOn.transform.position, 8f));
    
            }
        }
    }


    IEnumerator NarrativeItem()
    {
        AudioSource.PlayClipAtPoint(narrativeClip, cinematicCamera.transform.position);
        yield return new WaitForSeconds(11);
        cinematicCamera.gameObject.SetActive(false);
        playerCam.gameObject.SetActive(true);
        playerManager.isCinematicPlaying = false;

    }
}
