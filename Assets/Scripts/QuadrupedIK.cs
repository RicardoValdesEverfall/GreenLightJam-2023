using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class QuadrupedIK : MonoBehaviour
{
    [SerializeField] private PlayerLocomotion playerLocomotion;
    [SerializeField] private QuadrupedIK otherFoot;
    [SerializeField] private Transform body;
    [SerializeField] private Vector3 footOffset;
    [SerializeField] private float stepHeight;
    [SerializeField] private float stepLength;
    [SerializeField] private float stepDistance;

    private Vector3 newPosition;
    private Vector3 currentPosition;
    private Vector3 oldPosition;

    private Vector3 newNormal;
    private Vector3 currentNormal;
    private Vector3 oldNormal;

    private float footSpacing;
    private float lerp;

    private void Start()
    {
        playerLocomotion = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerLocomotion>();

        footSpacing = transform.localPosition.x;

        currentPosition = transform.position;
        newPosition = transform.position;
        oldPosition = transform.position;

        currentNormal = transform.up;
        newNormal = transform.up;
        oldNormal = transform.up;
    }

    private void Update()
    {
        transform.position = currentPosition;
        transform.up = currentNormal;

        Ray ray = new Ray(body.position + (body.right * footSpacing), Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit info, 10))
        {
            if (Vector3.Distance(newPosition, info.point) > stepDistance && !otherFoot.IsMoving() && lerp >= 1)
            {
                lerp = 0;
                int direction = body.InverseTransformPoint(info.point).z > body.InverseTransformPoint(newPosition).z ? 1 : -1;
                newPosition = info.point + (body.forward * stepLength * direction) + footOffset;
                newNormal = info.normal;
            }
        }

        if (lerp < 1)
        {
            Vector3 tempPosition = Vector3.Lerp(oldPosition, newPosition, lerp);
            tempPosition.y += Mathf.Sin(lerp * Mathf.PI) * stepHeight;

            currentPosition = tempPosition;
            currentNormal = Vector3.Lerp(oldNormal, newNormal, lerp);
            lerp += Time.deltaTime * playerLocomotion.currentSpeed;
        }
        else
        {
            oldPosition = newPosition;
            oldNormal = newNormal;
        }
    }

    public bool IsMoving()
    {
        return (lerp < 1);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.01f);
    }
}
