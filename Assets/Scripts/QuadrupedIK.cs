using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class QuadrupedIK : MonoBehaviour
{
    [Header("IK COMPONENTS")]
    [SerializeField] private TwoBoneIKConstraint[] footIKConstraints;
    [SerializeField] private Transform[] targetTransforms; //FL = 0, FR = 1, BL = 2, BR =3
    [SerializeField] private Transform[] footTransforms;
    [SerializeField] private LayerMask groundLayer;

    [Header("IK SETTINGS")]
    [SerializeField] private float stepHeight;
    [SerializeField] private float stepLength;
    [SerializeField] private float maxDistance;
    [SerializeField] private float sphereRadius;

    private float angleX;
    private float angleZ;

    private Vector3[] allHitNormals;

    private void Start()
    {
        allHitNormals = new Vector3[4];
    }

    private void FixedUpdate()
    {
        HandleFeetRotation();
    }

    private Vector3 HandleContactPlane(Vector3 plane, Vector3 hitNormal)
    {
        return plane - hitNormal * Vector3.Dot(plane, hitNormal);
    }

    private void HandleAxisAngles(Transform footTarget, Vector3 hitNormal)
    {
        Vector3 xAxisProject = HandleContactPlane(footTarget.forward, hitNormal).normalized;
        Vector3 zAxisProject = HandleContactPlane(footTarget.right, hitNormal).normalized;

        angleX = Vector3.SignedAngle(footTarget.forward, xAxisProject, footTarget.right);
        angleZ = Vector3.SignedAngle(footTarget.right, zAxisProject, footTarget.forward);
    }

    private void HandleFeetRotation()
    {
        for(int i = 0; i < 4; i++)
        {
            RaycastHit hit;

            if (Physics.SphereCast(footTransforms[i].position, sphereRadius, Vector3.down, out hit, groundLayer))
            {
                allHitNormals[i] = hit.normal;

                HandleAxisAngles(footTransforms[i], hit.normal);
                targetTransforms[i].position = hit.point;
                targetTransforms[i].localEulerAngles = new Vector3(targetTransforms[i].localEulerAngles.x + angleX, targetTransforms[i].localEulerAngles.y, targetTransforms[i].localEulerAngles.z + angleZ);
              
            }
        }
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < 4; i++)
        {
            Gizmos.DrawSphere(footTransforms[i].position, sphereRadius);
        }
       
    }
}
