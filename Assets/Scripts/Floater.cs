using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Floater : MonoBehaviour
{
    [SerializeField] float altitude;
    [SerializeField] float time;
    // Start is called before the first frame update
    void Start()
    {
        transform.DOMoveY(transform.position.y + altitude, time).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
