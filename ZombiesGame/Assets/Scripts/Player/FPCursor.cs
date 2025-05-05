using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FP_Cursor : MonoBehaviour
{
    public float mouseSensitivity;
    float xRotation = 0;
    public Camera playerCamera;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90,90);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation,0,0);
        transform.Rotate(Vector3.up*mouseX);
    }
}
