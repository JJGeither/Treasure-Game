using UnityEngine;

public class DroneScript : MonoBehaviour
{
    [Header("Boid Variables")]
    public float followHomeWeight = 1.0f; // Weight of following home position behavior
    public float maxSpeed = 5.0f; // Maximum speed of the drone
    public float followHomeRadius = 10.0f; // Radius to consider home position for following
    public float circleRadius = 5.0f; // Radius for circling around the home position
    public float minDistance = 2.0f; // Minimum distance from the home position
    public float avoidanceRadius = 3.0f; // Radius to consider neighboring drones for avoidance
    public float avoidanceWeight = 1.0f; // Weight of avoidance behavior

    private Vector3 velocity; // Current velocity of the drone
    private Vector3 acceleration; // Current acceleration of the drone
    private Vector3 homePosition; // Home position for the drone to follow

    private bool droneStarted = false;

    void Update()
    {
        if (droneStarted)
        {
            UpdateHomePosition();
            CalculateDesiredVelocity();
            UpdatePosition();
        }
    }

    public void DroneStartup()
    {
        Destroy(this.GetComponent<Interactor>());
        UpdateHomePosition(); // Set initial home position
        transform.position = homePosition;
        droneStarted = true;
    }

    private void UpdateHomePosition()
    {
        // Follows the player character
        homePosition = PlayerController.instance.transform.position + Vector3.up * 3f;
    }

    private void CalculateDesiredVelocity()
    {
        // Calculate direction to home position
        Vector3 directionToHome = homePosition - transform.position;
        directionToHome.y = 0f; // Ignore Y component

        // Calculate perpendicular vector for circling around home position
        Vector3 perpendicular = Vector3.Cross(directionToHome.normalized, Vector3.up);

        // Calculate desired position for circling around home position
        Vector3 desiredPosition = homePosition + perpendicular * circleRadius;

        // Calculate direction to desired position
        Vector3 directionToDesired = desiredPosition - transform.position;

        // Calculate distance to home position
        float distanceToHome = directionToHome.magnitude;

        // Check if the drone is too close to the home position
        if (distanceToHome < minDistance)
        {
            // Move away from home position
            directionToHome *= -1f;
        }

        // Calculate desired velocity towards the desired position
        Vector3 desiredVelocity = directionToDesired.normalized * maxSpeed;

        // Calculate avoidance behavior
        Vector3 avoidance = CalculateAvoidance();

        // Combine behaviors with respective weights
        desiredVelocity += avoidance * avoidanceWeight;

        // Limit desired velocity to maximum speed
        desiredVelocity = Vector3.ClampMagnitude(desiredVelocity, maxSpeed);

        // Assign the desired velocity to acceleration
        acceleration = desiredVelocity - velocity;
    }

    private Vector3 CalculateAvoidance()
    {
        Vector3 avoidance = Vector3.zero;
        Collider[] colliders = Physics.OverlapSphere(transform.position, avoidanceRadius);

        foreach (var collider in colliders)
        {
            if (collider != null && collider != GetComponent<Collider>())
            {
                Vector3 avoidanceDirection = transform.position - collider.transform.position;
                avoidance += avoidanceDirection.normalized / avoidanceDirection.magnitude;
            }
        }

        return avoidance;
    }

    private void UpdatePosition()
    {
        // Update velocity and position based on acceleration
        velocity += acceleration * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
        transform.position += velocity * Time.deltaTime;

        // Rotate drone to face its velocity direction
        if (velocity != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(velocity, Vector3.up);
        }
    }
}
