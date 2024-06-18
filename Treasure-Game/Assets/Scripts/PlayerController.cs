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

    [Header("Raycast Settings")]
    [SerializeField] private float sphereRadius;

    [Header("Layer Settings")]
    [SerializeField] private LayerMask groundLayer;

    [Header("Money Variables")]
    public  float moneyAmount = 0;

    [Header("Drone Variables")]
    public List<DroneScript> followingDrones = new List<DroneScript>();

    [Header("Script References")]
    public Interactor interactor;


    private float _playerSpeed;
    private float verticalInput;
    private float horizontalInput;
    private Rigidbody _rb;

    void Awake() => instance = this;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        interactor = GetComponent<Interactor>();
    }

    void Update()
    {
        ObtainPlayerMovementInput();

        CheckIfTouchingGround();

        RotatePlayer();
    }

    private void FixedUpdate()
    {
        // Applies the movement from user input and additional downward force
        ApplyPlayerMovement();
    }

    void RotatePlayer()
    {
        float mouseX = Input.GetAxis("Mouse X");
        mouseX = (mouseX + 180.0f) % 360.0f - 180.0f;

        Quaternion targetRotation = Quaternion.Euler(0, transform.eulerAngles.y + mouseX, 0);
        _rb.MoveRotation(targetRotation);
    }

    void ObtainPlayerMovementInput()
    {
        // Get player inputs
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");

        // Calculate target speed
        if (verticalInput != 0.0f || horizontalInput != 0.0f)
            _playerSpeed += acceleration;
        else
            _playerSpeed -= deceleration;

        // Clamp the target speed
        _playerSpeed = Mathf.Max(_playerSpeed, 0f);
        _playerSpeed = Mathf.Clamp(_playerSpeed, playerMinSpeed, playerMaxSpeed);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.down * (sphereRadius * 1.51f), sphereRadius);
    }

    void CheckIfTouchingGround()
    {
        // Perform the raycast
        if (!Physics.CheckSphere(transform.position + Vector3.down * (sphereRadius * 1.51f), sphereRadius, groundLayer))
        {
            Debug.Log("Not touching ground");
            _rb.AddForce(Vector3.down * additionalGravity, ForceMode.Acceleration);
        }
    }

    void ApplyPlayerMovement()
    {
        float verticalMovement = verticalInput * _playerSpeed;
        float horizontalMovement = horizontalInput * _playerSpeed;

        Vector3 movement = new Vector3(horizontalMovement, 0.0f, verticalMovement);

        movement = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0) * movement;

        Vector3 totalVelocity = movement;

        // Apply the calculated velocity
        _rb.velocity = totalVelocity;
    }
}
