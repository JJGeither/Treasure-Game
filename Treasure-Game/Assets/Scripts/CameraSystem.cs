using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    [Header("Public Variables")]
    [SerializeField] private Transform player;
    [SerializeField] private Vector3 offset = new Vector3(0, 2, -5);
    [SerializeField] private float followSpeed = 10f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float minYRotation = -40f; // Minimum angle for camera rotation
    [SerializeField] private float maxYRotation = 20f; // Maximum angle for camera rotation
    [SerializeField] private float minDistance = 2f; // Minimum distance from player
    [SerializeField] private float maxDistance = 40f; // Maximum distance from player
    [SerializeField] private float zoomSpeed = 5f;

    private float currentYRotation = 0f;
    private float currentDistance = 5f;

    // Update is called once per frame
    void Update()
    {
        float mouseY = Input.GetAxis("Mouse Y");
        currentYRotation -= mouseY * rotationSpeed * Time.deltaTime;
        currentYRotation = Mathf.Clamp(currentYRotation, minYRotation, maxYRotation);
        Debug.Log(currentYRotation);

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        currentDistance -= scroll * zoomSpeed;
        currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);

        // Calculate the rotation around the player based on mouseY input
        Quaternion rotation = Quaternion.Euler(currentYRotation, player.eulerAngles.y, 0f);

        // Calculate the new offset based on the zoom distance
        Vector3 zoomOffset = offset.normalized * currentDistance;

        // Smoothly interpolate the camera's position
        Vector3 targetPosition = player.position + rotation * zoomOffset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        // Look at the player
        transform.LookAt(player.position);
    }
}
