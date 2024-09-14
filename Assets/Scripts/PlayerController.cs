using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// To auto add CharacterController to GameObject
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public Camera playerCam;

    // Player Controller
    CharacterController characterController;

    // Camera Settings:
    public float lookSpeed = 2f;
    public float lookXLimit = 75f;
    public float cameraRotationSmooth = 5f;

    // Camera Zoom Settings:
    public int ZoomFOV = 50;
    public int initialFOV = 80;
    public float cameraZoomSmooth = 1;
    private bool isZoomed = false;

    // Moving Properties
    public float walkSpeed = 3f;
    public float runSpeed = 20f;
    public float jumpPower = 0f;
    public float gravity = 10f;
    // Can The Player Move
    private bool canMove = true;

    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;
    float rotationY = 0;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        // To lock and hide cursor for shooting and playing
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Walking
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        // Running
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Check if player is Running
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);

        // Camera for Movement :
        if (canMove)
        {
            rotationX -= Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

            rotationY += Input.GetAxis("Mouse X") * lookSpeed;

            Quaternion targetRotationX = Quaternion.Euler(rotationX, 0, 0);
            Quaternion targetRotationY = Quaternion.Euler(0, rotationY, 0);

            playerCam.transform.localRotation = Quaternion.Slerp(playerCam.transform.localRotation, targetRotationX, Time.deltaTime * cameraRotationSmooth);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotationY, Time.deltaTime * cameraRotationSmooth);
        }

        // Zooming:
        if (Input.GetButtonDown("Fire2"))
        {
            isZoomed = true;
        }
        if (Input.GetButtonUp("Fire2"))
        {
            isZoomed = false;
        }
        float cameraZoom = Time.deltaTime * cameraZoomSmooth;
        if (isZoomed)
        {
            playerCam.GetComponent<Camera>().fieldOfView = Mathf.Lerp(playerCam.fieldOfView, ZoomFOV, cameraZoom);
        }
        else
        {
            playerCam.GetComponent<Camera>().fieldOfView = Mathf.Lerp(playerCam.fieldOfView, initialFOV, cameraZoom);
        }
    }
}
