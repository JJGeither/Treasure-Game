using UnityEngine;

public class DroneScript : MonoBehaviour
{
    [Header("Boid Variables")]
    public float followHomeWeight = 1.0f;
    public float maxSpeed = 15.0f;
    public float followHomeRadius = 10.0f;
    public float circleRadius = 5.0f;
    public float minDistance = 2.0f;
    public float avoidanceRadius = 3.0f;
    public float avoidanceWeight = 1.0f;

    private Vector3 velocity;
    private Vector3 acceleration;
    private Vector3 homePosition;

    private bool droneStarted = false;

    public bool isBusy { get; private set; } = false;
    public Interactor interactor { get; private set; }

    void Start()
    {
        interactor = GetComponent<Interactor>();
    }

    void Update()
    {
        if (droneStarted)
        {
            if (isBusy)
            {
                MoveToHomePosition();
            }
            else
            {
                UpdateHomePosition();
                CalculateDesiredVelocity();
                UpdatePosition();
            }
        }
    }

    public void DroneStartup()
    {
        Destroy(GetComponent<Interactee>());
        UpdateHomePosition();
        transform.position = homePosition;
        droneStarted = true;
    }

    private void UpdateHomePosition()
    {
        homePosition = PlayerController.instance.transform.position + Vector3.up * 3f;
    }

    private void CalculateDesiredVelocity()
    {
        Vector3 directionToHome = (homePosition - transform.position).normalized;
        directionToHome.y = 0f;

        Vector3 perpendicular = Vector3.Cross(directionToHome, Vector3.up);
        Vector3 desiredPosition = homePosition + perpendicular * circleRadius;
        Vector3 directionToDesired = (desiredPosition - transform.position).normalized;

        float distanceToHome = Vector3.Distance(transform.position, homePosition);

        if (distanceToHome < minDistance)
        {
            directionToHome *= -1f;
        }

        Vector3 desiredVelocity = directionToDesired * maxSpeed;
        Vector3 avoidance = CalculateAvoidance();

        desiredVelocity += avoidance * avoidanceWeight;
        desiredVelocity = Vector3.ClampMagnitude(desiredVelocity, maxSpeed);

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
                Vector3 avoidanceDirection = (transform.position - collider.transform.position).normalized;
                avoidance += avoidanceDirection / avoidanceDirection.magnitude;
            }
        }

        return avoidance;
    }

    private void UpdatePosition()
    {
        float distanceToHome = Vector3.Distance(transform.position, homePosition);

        if (distanceToHome < minDistance)
        {
            velocity = Vector3.zero;
        }

        velocity = Vector3.ClampMagnitude(velocity + acceleration * Time.deltaTime, maxSpeed);
        transform.position += velocity * Time.deltaTime;

        if (velocity != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(velocity, Vector3.up);
        }
    }

    private void MoveToHomePosition()
    {
        if (HasReachedHome())
        {
            velocity = Vector3.zero;
            isBusy = false;
            return;
        }

        Vector3 directionToHome = (homePosition - transform.position).normalized;
        Vector3 desiredVelocity = directionToHome * maxSpeed;
        Vector3 avoidance = CalculateAvoidance();

        desiredVelocity += avoidance * avoidanceWeight;
        desiredVelocity = Vector3.ClampMagnitude(desiredVelocity, maxSpeed);

        acceleration = desiredVelocity - velocity;

        velocity = Vector3.ClampMagnitude(velocity + acceleration * Time.deltaTime, maxSpeed);
        transform.position += velocity * Time.deltaTime;

        if (velocity != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(velocity, Vector3.up);
        }
    }

    public bool HasReachedHome()
    {
        float distanceToHome = Vector3.Distance(transform.position, homePosition);
        return distanceToHome < minDistance;
    }

    public void MoveDroneTo(Vector3 travelPosition)
    {
        isBusy = true;
        homePosition = travelPosition;
    }
}
