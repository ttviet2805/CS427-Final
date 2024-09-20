using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour {
    // Camera Properties
    public Camera playerCamera;
    public float lookSpeed = 2f;
    public float lookXLimit = 75f;
    public float cameraRotationSmooth = 5f;

    // Camera Zoom Properties
    public int ZoomFOV = 50;
    public int initialFOV = 80;
    public float cameraZoomSmooth = 1;
    private bool isZoomed = false;

    // Player Controller
    CharacterController characterController;

    // Player Properties
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;
    float rotationY = 0;

    public float walkSpeed = 3f;
    public float runSpeed = 20f;
    public float defaultGravity = 10f;

    private bool canMove = true;

    // Audio Sounds
    public AudioClip[] woodFootstepSounds;
    public Transform footstepAudioPosition;
    public AudioSource audioSource;

    private bool isWalking = false;
    private bool isFootstepCoroutineRunning = false;
    private AudioClip[] currentFootstepSounds;

    void Start() {
        characterController = GetComponent <CharacterController>();

        // Cursor Start Game Setting
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update() {
        // Player Movement
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        Vector3 zDir = transform.TransformDirection(Vector3.forward);
        Vector3 xDir = transform.TransformDirection(Vector3.right);

        float curSpeed = (isRunning ? runSpeed : walkSpeed);
        float zSpeed = canMove ? curSpeed * Input.GetAxis("Vertical") : 0;
        float xSpeed = canMove ? curSpeed * Input.GetAxis("Horizontal") : 0;
        moveDirection = (zDir * zSpeed) + (xDir * xSpeed); 

        // If player is higher than the ground floor -> Let gravity push player down
        if (!characterController.isGrounded) {
            moveDirection.y -= defaultGravity * Time.deltaTime;
        }
        characterController.Move(moveDirection * Time.deltaTime);

        // Camera Setting
        if (canMove) {
            rotationX -= Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

            rotationY += Input.GetAxis("Mouse X") * lookSpeed;

            Quaternion targetRotationX = Quaternion.Euler(rotationX, 0, 0);
            Quaternion targetRotationY = Quaternion.Euler(0, rotationY, 0);

            playerCamera.transform.localRotation = Quaternion.Slerp(playerCamera.transform.localRotation, targetRotationX, Time.deltaTime * cameraRotationSmooth);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotationY, Time.deltaTime * cameraRotationSmooth);
        }

        // Zoom Action:
        if (Input.GetButtonDown("Fire2")) isZoomed = true;
        if (Input.GetButtonUp("Fire2")) isZoomed = false;

        if (isZoomed)
            playerCamera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, ZoomFOV, Time.deltaTime * cameraZoomSmooth);
        else
            playerCamera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, initialFOV, Time.deltaTime * cameraZoomSmooth);
    
        // Walking and Running Sound
        if ((zSpeed != 0f || xSpeed != 0f) && !isWalking && !isFootstepCoroutineRunning) {
            isWalking = true;
            StartCoroutine(PlayFootstepSounds(1.3f / (isRunning ? runSpeed : walkSpeed)));
        }
        else if (zSpeed == 0f && xSpeed == 0f) {
            isWalking = false;
        }
    }

    IEnumerator PlayFootstepSounds(float footstepDelay) {
        isFootstepCoroutineRunning = true;
        while (isWalking) {
            if (currentFootstepSounds.Length > 0) {
                int randomIndex = Random.Range(0, currentFootstepSounds.Length);
                audioSource.transform.position = footstepAudioPosition.position;
                audioSource.clip = currentFootstepSounds[randomIndex];
                audioSource.Play();
                yield return new WaitForSeconds(footstepDelay);
            }
            else yield break;
        }
        isFootstepCoroutineRunning = false;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Wood"))
            currentFootstepSounds = woodFootstepSounds;
    }
}