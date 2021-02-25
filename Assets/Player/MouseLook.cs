using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBody;
    float xRotation = -10f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    float GetDeltaFactor(float delta)
    {
        return delta / (1/16f);
    }
    
    void Update()
    {
        float deltaFactor = GetDeltaFactor(Time.deltaTime);
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * deltaFactor;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * deltaFactor;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -70, 70f);
        // Debug.Log(xRotation);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
