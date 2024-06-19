using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    [Header("Movement Variables")]
    [SerializeField] public float playerMaxSpeed;
    [SerializeField] public float playerMinSpeed;
    [SerializeField] public float acceleration;
    [SerializeField] public float deceleration;
    [SerializeField] public float additionalGravity = 10f;
    [SerializeField] public float jumpHeight;

    [Header("Raycast Settings")]
    [SerializeField] private float sphereRadius;

    [Header("Layer Settings")]
    [SerializeField] private LayerMask groundLayer;

    [Header("Money Variables")]
    public float moneyAmount = 0;

    [Header("Drone Variables")]
    public List<DroneScript> followingDrones = new List<DroneScript>();

    [Header("Oxygen Variables")]
    public float OxygenLevel;
    public float OxygenMaxLevel;

    private IEnumerator DecreaseOxygenLevel()
    {
        while (OxygenLevel > 0)
        {
            yield return new WaitForSeconds(10);
            OxygenLevel -= 1;
            Debug.Log("Oxygen Level: " + OxygenLevel);
        }

        // Handle what happens when oxygen level reaches zero, if needed
        // For example, you might call a method to handle player death or other game logic
        // PlayerDeath();
    }

    [Header("Script References")]
    public Interactor interactor;

    private float _playerSpeed;
    private float verticalInput;
    private float horizontalInput;

    // Jumping Inputs
    private bool jumpInput;
    private bool isJumping = false;
    private float jumpForce = 0.0f;
    private bool isGrounded = false;
    private Rigidbody _rb;

    void Awake() => instance = this;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        interactor = GetComponent<Interactor>();
        OxygenLevel = OxygenMaxLevel;
        StartCoroutine(DecreaseOxygenLevel());
    }

    void Update()
    {
        ObtainPlayerMovementInput();
        CheckIfTouchingGround();
        RotatePlayer();
    }

    void FixedUpdate()
    {
        ApplyPlayerMovement();
    }

    private void ObtainPlayerMovementInput()
    {
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");
        jumpInput = Input.GetKey(KeyCode.LeftShift);

        if (verticalInput != 0.0f || horizontalInput != 0.0f)
            _playerSpeed += acceleration;
        else
            _playerSpeed -= deceleration;

        _playerSpeed = Mathf.Max(_playerSpeed, 0f);
        _playerSpeed = Mathf.Clamp(_playerSpeed, playerMinSpeed, playerMaxSpeed);
    }

    private void RotatePlayer()
    {
        float mouseX = Input.GetAxis("Mouse X");
        mouseX = (mouseX + 180.0f) % 360.0f - 180.0f;

        Quaternion targetRotation = Quaternion.Euler(0, transform.eulerAngles.y + mouseX, 0);
        _rb.MoveRotation(targetRotation);
    }

    private void CheckIfTouchingGround()
    {
        if (!Physics.CheckSphere(transform.position + Vector3.down * (sphereRadius * 1.51f), sphereRadius, groundLayer))
        {
            isGrounded = false;
            if (!isJumping)
            {
                _rb.AddForce(Vector3.down * additionalGravity, ForceMode.Acceleration);
            }

        } else
        {
            isGrounded = true;
        }
    }

    private void JumpPhysics()
    {
        DetermineIfJumping();
        ApplyJumpForce();
        ReduceJumpOvertime();
    }

    private void DetermineIfJumping()
    {
        if (jumpInput && isGrounded)
        {
            isJumping = true;
            jumpForce = jumpHeight;
            Debug.Log("Space key pressed");
        }
    }

    private void ReduceJumpOvertime()
    {
        if (isJumping)
        {
            if (!jumpInput)
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
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Acceleration);
        }
    }

    private void ApplyPlayerMovement()
    {
        float verticalMovement = verticalInput * _playerSpeed;
        float horizontalMovement = horizontalInput * _playerSpeed;

        Vector3 movement = new Vector3(horizontalMovement, 0.0f, verticalMovement);
        movement = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0) * movement;

        _rb.velocity = movement;

        JumpPhysics();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.down * (sphereRadius * 1.51f), sphereRadius);
    }
}
