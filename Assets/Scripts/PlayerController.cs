using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("Player controls")]
    [SerializeField] Camera playerCamera;
    [SerializeField] CharacterController characterController;
    [SerializeField] InputActionAsset playerControls;
    [SerializeField] float playerSpeed = 1.0f;
    [SerializeField] float mouseSensitivity = 2.0f;
    [SerializeField] float upDownRange = 80.0f;
    [SerializeField] float gravity = 9.81f;
    InputAction moveAction;
    InputAction lookAction;
    InputAction interactAction;
    Vector2 moveInput;
    Vector2 lookInput;
    Vector3 currentMovement = Vector3.zero;
    bool isMoving;
    float verticalRotation;
    LayerMask layerMask;
    [Header("Audio controls")]
    [SerializeField] AudioSource stepAudio;
    [SerializeField] float stepInterval = 0.5f;
    [SerializeField] float velocityThreshold = 2.0f;
    [SerializeField] AudioClip[] audioClips;
    float nextStepTime;
    int lastPlayedIndex = -1;
    [Header("UI controls")]
    [SerializeField] Volume volume;
    [SerializeField] RawImage rawImage;
    [SerializeField] Animator equipAnimator;
    [SerializeField] TMP_Text interactText;


    void Awake()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
        if (characterController == null)
        {
            gameObject.GetComponent<CharacterController>();
        }
        if (stepAudio == null)
        {
            gameObject.GetComponent<AudioSource>();
        }
        moveAction = playerControls.FindActionMap("Player").FindAction("Move");
        lookAction = playerControls.FindActionMap("Player").FindAction("Look");
        interactAction = playerControls.FindActionMap("Player").FindAction("Interact");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        moveAction.performed += context => moveInput = context.ReadValue<Vector2>();
        moveAction.canceled += context => moveInput = Vector2.zero;

        lookAction.performed += context => lookInput = context.ReadValue<Vector2>();
        lookAction.canceled += context => lookInput = Vector2.zero;

        layerMask = LayerMask.GetMask("Interactable");
    }

    void Onable()
    {
        moveAction.Enable();
        lookAction.Enable();
    }
    void OnDisable()
    {
        moveAction.Disable();
        lookAction.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleInteraction();
        HandleFootsteps();
        HandleInteractableText();
    }
    void HandleMovement()
    {
        float verticalSpeed = moveInput.y * playerSpeed;
        float horizontalSpeed = moveInput.x * playerSpeed;

        Vector3 horizontalMovement = new Vector3(horizontalSpeed, 0, verticalSpeed);
        HandleGravity();
        horizontalMovement = transform.rotation * horizontalMovement;
        currentMovement.x = horizontalMovement.x;
        currentMovement.z = horizontalMovement.z;

        characterController.Move(currentMovement * Time.deltaTime);

        isMoving = moveInput.y != 0 || moveInput.x != 0;
    }
    void HandleRotation()
    {
        float mouseXRotation = lookInput.x * mouseSensitivity;
        transform.Rotate(0, mouseXRotation, 0);

        verticalRotation -= lookInput.y * mouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -upDownRange, upDownRange);
        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }
    void HandleGravity()
    {
        if (!characterController.isGrounded)
        {
            currentMovement.y -= gravity * Time.deltaTime;
        }
    }
    void HandleFootsteps()
    {
        if (characterController.isGrounded && isMoving && Time.time > nextStepTime && characterController.velocity.magnitude > velocityThreshold)
        {
            PlayFootstepSounds();
            nextStepTime = Time.time + stepInterval;
        }
    }
    void HandleInteraction()
    {
        if (interactAction.triggered)
        {
            RaycastHit hit;
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.TransformDirection(Vector3.forward), out hit, 1.5f, layerMask))
            {
                equipAnimator.SetTrigger("equip");
                Texture overlay;
                Debug.Log("click");
                hit.collider.gameObject.GetComponentInParent<MaskController>().OnInteract(volume, out overlay);
                rawImage.texture = overlay;
            }
            else
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
                Debug.Log("Did not Hit");
            }

        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish"))
        {
            Debug.Log("you win");
        }
    }
    void PlayFootstepSounds()
    {
        if (stepAudio == null)
        {
            Debug.Log("step");
        }
        else if (stepAudio != null && audioClips == null)
        {
            stepAudio.PlayOneShot(stepAudio.clip);
        }
        else if (stepAudio != null && audioClips != null)
        {
            int indexToUse = -1;
            bool loop = true;
            if (audioClips.Length > 1)
            {
                while (loop)
                {
                    indexToUse = UnityEngine.Random.Range(0, audioClips.Length - 1);
                    if (indexToUse != lastPlayedIndex)
                    {
                        lastPlayedIndex = indexToUse;
                        loop = false;
                    }
                }
                stepAudio.PlayOneShot(audioClips[indexToUse]);
            }

        }
    }
    void HandleInteractableText()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.TransformDirection(Vector3.forward), out hit, 1.5f, layerMask))
        {
            if (!interactText.gameObject.activeSelf)
            {
                interactText.gameObject.SetActive(true);
            }
        }
        else
        {
            if (interactText.gameObject.activeSelf)
            {
                interactText.gameObject.SetActive(false);
            }
        }
    }
}
