using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class GameManager : MonoBehaviour
{
    [SerializeField] PlayerManager player;
    [SerializeField] Camera cinematicCamera;

    [Header("Room cinematic")]
    [SerializeField] private GameObject door;
    [SerializeField] private Transform doorPivot;


    private Camera mainCam;
    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
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

        seq.Append(cinematicCamera.transform.DOMove(doorPivot.position,2.5f));
        seq.Join(cinematicCamera.transform.DORotate(doorPivot.rotation.eulerAngles, 2.5f));
        seq.Append(door.transform.DOLocalRotate(new Vector3(-90, 0, -8.253f), 2f));
        seq.Join(door.transform.DOLocalMove(new Vector3(-1.19806147f, 0, 0.978580475f), 2f));
        seq.SetEase(Ease.InOutSine);

        seq.OnComplete(() =>
        {
            cinematicCamera.gameObject.SetActive(false);
        });
        
    }
}
