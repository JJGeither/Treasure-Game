using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Public Variables")]
    [SerializeField] public float playerMaxSpeed;
    [SerializeField] public float playerMinSpeed;
    [SerializeField] public float acceleration;
    [SerializeField] public float deceleration;


    private float _playerSpeed;
    private float _targetSpeed;
    private float _speedSmoothVelocity;
    private float _jumpForce = 0;
    private float verticalInput;
    private float horizontalInput;
    private Rigidbody _rb;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Get player inputs
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");

        // Calculate target speed
        if (verticalInput != 0.0f || horizontalInput != 0.0f)
            _targetSpeed += acceleration;
        else
            _targetSpeed -= deceleration;

        // Clamp the target speed
        _targetSpeed = Mathf.Clamp(_targetSpeed, playerMinSpeed, playerMaxSpeed);

        // Get mouse X input for rotation
        float mouseX = Input.GetAxis("Mouse X");
        RotatePlayer(mouseX);


    }

    private void FixedUpdate()
    {
        // Smoothly adjust the playerSpeed towards the targetSpeed
        _playerSpeed = Mathf.SmoothDamp(_playerSpeed, _targetSpeed, ref _speedSmoothVelocity, acceleration);

        float verticalMovement = verticalInput * _playerSpeed;
        float horizontalMovement = horizontalInput * _playerSpeed;

        Vector3 movement = new Vector3(horizontalMovement, 0.0f, verticalMovement);

        movement = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0) * movement;

        Vector3 totalVelocity = movement + Vector3.up * _jumpForce;
        _rb.velocity = totalVelocity;
    }

    void RotatePlayer(float mouseX)
    {
        mouseX = (mouseX + 180.0f) % 360.0f - 180.0f;
        Quaternion targetRotation = Quaternion.Euler(0, transform.eulerAngles.y + mouseX, 0);
        _rb.MoveRotation(targetRotation);
    }
}

