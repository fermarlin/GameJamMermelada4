using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5;
    public float rotationSpeed = 300;

    public float jumpForce = 5;
    public float jumpTime = 0.35f;
    private float jumpTimeCounter;
    private bool isJumping;
    public bool isPressingJumping=false;

    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask whatIsGround;

    public bool grounded;

    public GameObject playerMesh;

    private Rigidbody playerRb;

    private Transform mainCamera;

    private PlayerInput playerInput;
    private @PlayerInputsActions playerInputActions;


    private void Awake()
    {
        playerRb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();

        playerInputActions = new @PlayerInputsActions();
        playerInputActions.Player.Enable();

        playerInputActions.Player.Jump.performed += Jump;
        playerInputActions.Player.Jump.canceled += Jump;
        Cursor.visible = false;
    }

    private void Start()
    {
        jumpTimeCounter = jumpTime;
    }

    private void Update()
    {
        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            playerInputActions.Player.Disable();
            playerInputActions.UI.Enable();
        }

        if (Keyboard.current.yKey.wasPressedThisFrame)
        {
            playerInputActions.Player.Enable();
            playerInputActions.UI.Disable();
        }

        Collider[] groundColliders = Physics.OverlapSphere(groundCheck.position, groundCheckRadius, whatIsGround);
        grounded = groundColliders.Length > 0;

        Collider[] movingCollider = Physics.OverlapSphere(groundCheck.position, groundCheckRadius);


    }

    private void FixedUpdate()
    {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
        Vector3 directionMovement = new Vector3(inputVector.x, 0, inputVector.y);

        mainCamera = Camera.main.transform;
        directionMovement = mainCamera.TransformDirection(directionMovement);
        directionMovement = new Vector3(directionMovement.x, 0, directionMovement.z);

        Vector3 targetVelocity = directionMovement * moveSpeed;
        Vector3 forceToAdd = (targetVelocity - playerRb.velocity) * moveSpeed * 0.5f;
        forceToAdd.y = 0;

        playerRb.AddForce(forceToAdd);

        if (directionMovement != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(directionMovement, Vector3.up);
            playerMesh.transform.rotation = Quaternion.RotateTowards(playerMesh.transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }

        if (grounded && !isJumping)
        {
            jumpTimeCounter = jumpTime;
        }


        if (isJumping){
            if (grounded && jumpTimeCounter==jumpTime)
            {
                playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
            else if (isPressingJumping && jumpTimeCounter > 0)
            {
                playerRb.velocity = new Vector3(playerRb.velocity.x, jumpForce, playerRb.velocity.z);
            }
        }

        if (!grounded)
        {
            playerRb.velocity = new Vector3(playerRb.velocity.x, playerRb.velocity.y - 30f * Time.deltaTime, playerRb.velocity.z);
            jumpTimeCounter -= Time.deltaTime;

        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isJumping = true;
            if (grounded)
            {
                isPressingJumping = true;
            }

        }
        if (context.canceled)
        {
            isJumping = false;
            isPressingJumping = false;

        }
    }


}