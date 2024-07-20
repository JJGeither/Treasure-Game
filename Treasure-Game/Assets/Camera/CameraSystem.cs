using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    [Header("Public Variables")]
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Transform player;
    [SerializeField] private Vector3 offset = new Vector3(0, 2, -5);
    [SerializeField] private float followSpeed = 10f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float minYRotation = -40f; // Minimum angle for camera rotation
    [SerializeField] private float maxYRotation = 20f; // Maximum angle for camera rotation
    [SerializeField] private float minDistance = 2f; // Minimum distance from player
    [SerializeField] private float maxDistance = 40f; // Maximum distance from player
    [SerializeField] private float zoomSpeed = 5f;

    public float currentYRotation = 0f;
    private float currentDistance = 5f;

    private RaycastHit hitInfo; // To store the raycast hit information

    // Update is called once per frame
    void Update()
    {
        // Handle camera rotation based on mouse input
        float mouseY = Input.GetAxis("Mouse Y");
        currentYRotation -= mouseY * rotationSpeed * Time.deltaTime;
        currentYRotation = Mathf.Clamp(currentYRotation, minYRotation, maxYRotation);

        // Handle camera zoom based on mouse scrollwheel input
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        currentDistance -= scroll * zoomSpeed;
        currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);

        // Calculate the rotation around the player based on mouseY input
        Quaternion rotation = Quaternion.Euler(currentYRotation, player.eulerAngles.y, 0f);

        // Calculate the new offset based on the zoom distance
        Vector3 zoomOffset = offset.normalized * currentDistance;

        // Calculate the target position of the camera
        Vector3 targetPosition = player.position + rotation * zoomOffset;

        // Perform a raycast from player to targetPosition to check for obstacles
        // Ignore triggers when performing the raycast

        if (Physics.Raycast(player.position, targetPosition - player.position, out hitInfo, Vector3.Distance(player.position, targetPosition), wallLayer))
        {
            // Adjust camera position to avoid clipping
            targetPosition = hitInfo.point - (targetPosition - player.position).normalized * 0.5f;
        }

        // Smoothly interpolate the camera's position towards the target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        // Make the camera look at the player's position
        transform.LookAt(player.position);
    }
}
