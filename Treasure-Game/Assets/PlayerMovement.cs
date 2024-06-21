using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Variables")]
    [SerializeField] private float playerMaxSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float deceleration;
    [SerializeField] private float additionalGravity = 10f;
    [SerializeField] private float jumpHeight;

    [Header("Raycast Settings")]
    [SerializeField] private float sphereRadius;
    [SerializeField] private float sphereDownwardOffset = 1.6f;

    [Header("Layer Settings")]
    [SerializeField] private LayerMask groundLayer;

    private Interactor interactor;
    private InputHandler inputHandler;
    private Rigidbody rb;

    private float playerSpeed;
    private bool isJumping = false;
    private float jumpForce = 0.0f;
    private bool isGrounded = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        interactor = GetComponent<Interactor>();
        inputHandler = GetComponent<InputHandler>();
    }

    private void Update()
    {
        CheckIfTouchingGround();
        RotatePlayer();
    }

    private void FixedUpdate()
    {
        ApplyPlayerMovement();
    }

    private void RotatePlayer()
    {
        float mouseX = inputHandler.mouseX;
        mouseX = (mouseX + 180.0f) % 360.0f - 180.0f;

        Quaternion targetRotation = Quaternion.Euler(0, transform.eulerAngles.y + mouseX, 0);
        rb.MoveRotation(targetRotation);
    }

    private void CheckIfTouchingGround()
    {
        if (!Physics.CheckSphere(transform.position + Vector3.down * (sphereRadius * sphereDownwardOffset), sphereRadius, groundLayer))
        {
            isGrounded = false;
            if (!isJumping)
            {
                rb.AddForce(Vector3.down * additionalGravity, ForceMode.Acceleration);
            }
        }
        else
        {
            isGrounded = true;
        }
    }

    private void JumpPhysics()
    {
        DetermineIfJumping();
        ApplyJumpForce();
        ReduceJumpOverTime();
    }

    private void DetermineIfJumping()
    {
        if (inputHandler.jumpInput && isGrounded)
        {
            isJumping = true;
            jumpForce = jumpHeight;
            Debug.Log("Jump key pressed");
        }
    }

    private void ReduceJumpOverTime()
    {
        if (isJumping)
        {
            if (!inputHandler.jumpInput)
                jumpForce = jumpForce / 2;

            // Gradually decrease jumpForce to make it smoother
            jumpForce -= 15;
            Debug.Log(jumpForce);

            if (jumpForce <= 0)
            {
                isJumping = false;
                jumpForce = 0;
            }
        }
    }

    private void ApplyJumpForce()
    {
        if (isJumping)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Acceleration);
        }
    }

    private void ApplyPlayerMovement()
    {
        float verticalMovement = inputHandler.verticalInput * playerSpeed;
        float horizontalMovement = inputHandler.horizontalInput * playerSpeed;

        if (verticalMovement != 0.0f || horizontalMovement != 0.0f)
            playerSpeed += acceleration;
        else
            playerSpeed -= deceleration;

        playerSpeed = Mathf.Max(playerSpeed, 0f);
        playerSpeed = Mathf.Clamp(playerSpeed, 1f, playerMaxSpeed);

        Vector3 movement = new Vector3(horizontalMovement, 0.0f, verticalMovement);
        movement = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0) * movement;

        rb.velocity = movement;

        JumpPhysics();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.down * (sphereRadius * sphereDownwardOffset), sphereRadius);
    }
}
