using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Camera playerCamera;
    [SerializeField] CharacterController characterController;
    [SerializeField] InputActionAsset playerControls;
    [SerializeField] float playerSpeed = 1.0f;
    [SerializeField] float mouseSensitivity = 2.0f;
    [SerializeField] float upDownRange = 80.0f;
    InputAction moveAction;
    InputAction lookAction;
    InputAction interactAction;
    Vector2 moveInput;
    Vector2 lookInput;
    Vector3 currentMovement = Vector3.zero;
    bool isMoving;
    float verticalRotation;
    LayerMask layerMask;


    void Awake()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
        if (characterController == null)
        {
            GetComponent<CharacterController>();
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
    }
    void HandleMovement()
    {
        float verticalSpeed = moveInput.y * playerSpeed;
        float horizontalSpeed = moveInput.x * playerSpeed;

        Vector3 horizontalMovement = new Vector3(horizontalSpeed, 0, verticalSpeed);

        horizontalMovement = transform.rotation * horizontalMovement;
        currentMovement.x = horizontalMovement.x;
        currentMovement.z = horizontalMovement.z;

        characterController.Move(currentMovement * Time.deltaTime);
    }
    void HandleRotation()
    {
        float mouseXRotation = lookInput.x * mouseSensitivity;
        transform.Rotate(0, mouseXRotation, 0);

        verticalRotation -= lookInput.y * mouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -upDownRange, upDownRange);
        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }
    void HandleInteraction()
    {
        if (interactAction.triggered)
        {
            RaycastHit hit;
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
            {
                Debug.Log("click");

                // Debug.Log(hit.collider.gameObject.name);
                hit.collider.gameObject.GetComponentInParent<MaskController>().OnInteract();
            }
            else
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
                Debug.Log("Did not Hit");
            }

        }
    }
}
