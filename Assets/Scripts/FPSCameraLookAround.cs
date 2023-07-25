using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCameraLookAround : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float senseX;
    public float senseY;

    float horizontalInput;
    float verticalInput;
    float mouseX;
    float mouseY;
    float xRotation;
    float yRotation;

    Quaternion originalRotation;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        originalRotation = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        xRotation += Input.GetAxis("Mouse X") * senseX;
        yRotation += Input.GetAxis("Mouse Y") * senseY;

        Quaternion xQuaternion = Quaternion.AngleAxis(xRotation, Vector3.up);
        Quaternion yQuaternion = Quaternion.AngleAxis(yRotation, -Vector3.right);

        transform.localRotation = originalRotation * xQuaternion * yQuaternion;

        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;

        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
    }
}
