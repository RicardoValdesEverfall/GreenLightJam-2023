using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using DG.Tweening;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] PlayerManager player;
    [SerializeField] Camera cinematicCamera;

    [Header("Room cinematic")]
    [SerializeField] private CanvasGroup hud;
    [SerializeField] private TextMeshProUGUI UIMemories;
    private int memoriesFound = 0;

    [Header("Room cinematic")]
    [SerializeField] private GameObject door;
    [SerializeField] private Transform doorPivot;


    private Camera mainCam;
    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
        hud.alpha = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //narrative cinematics
    public void OpenRoomDoorCinematic()
    {
        cinematicCamera.transform.position = mainCam.transform.position;
        cinematicCamera.transform.rotation = mainCam.transform.rotation;
        cinematicCamera.gameObject.SetActive(true);
        Sequence seq = DOTween.Sequence();
        player.isCinematicPlaying = true;
        seq.Append(cinematicCamera.transform.DOMove(doorPivot.position,2.5f));
        seq.Join(cinematicCamera.transform.DORotate(doorPivot.rotation.eulerAngles, 2.5f));
        seq.Append(door.transform.DOLocalRotate(new Vector3(-90, 0, -8.253f), 2f));
        seq.Join(door.transform.DOLocalMove(new Vector3(-1.19806147f, 0, 0.978580475f), 2f));
        seq.SetEase(Ease.InOutSine);

        seq.OnComplete(() =>
        {
            cinematicCamera.gameObject.SetActive(false);
            player.isCinematicPlaying = false;
        });
        
    }

    public void UpdateMemoriesCount()
    {
        memoriesFound++;
        UIMemories.text = memoriesFound + " out of 4 memories";
        Sequence seq = DOTween.Sequence();
        seq.Append(hud.DOFade(1, 2f));
        seq.AppendInterval(3f);
        seq.Append(hud.DOFade(0, 2f));
        if(memoriesFound >= 4)
        {
            SceneManager.LoadScene(0);
        }
        else
        {

        }
    }
}
