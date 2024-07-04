using UnityEngine;

[RequireComponent(typeof(DroneStats))]
public class DroneController : MonoBehaviour
{
    [Header("Movement Parameters")]
    public float maxSpeed = 15.0f;
    public float minDistance = 2.0f;
    public float circleRadius = 5.0f;

    [Header("Avoidance Parameters")]
    public float avoidanceRadius = 3.0f;
    public float avoidanceWeight = 1.0f;

    private Vector3 velocity;
    private Vector3 acceleration;
    private Vector3 homePosition;
    public Interactor interactor { get; private set; }

    public DroneStats droneStats;
    public DroneStats.DroneStateMachine stateMachine { get; private set; }

    void Start()
    {
        droneStats = GetComponent<DroneStats>();
        stateMachine = new DroneStats.DroneStateMachine(this);
        interactor = GetComponent<Interactor>();
    }

    void Update()
    {
        if (stateMachine.IsActive)
        {
            stateMachine.ExecuteCurrentState();
        }
    }

    public void Initialize()
    {
        droneStats.GenerateRandomName();
        Destroy(GetComponent<Interactee>());
        UpdateHomePosition();
        transform.position = homePosition;
    }

    public string GetDroneName() => droneStats.DroneName;

    public void SetDestination(Vector3 destination)
    {
        homePosition = destination;
    }

    public void IdleMovement()
    {
        UpdateHomePosition();
        CalculateIdleMovement();
        UpdatePosition();
    }

    public void MoveToDestination()
    {
        if (HasReachedDestination())
        {
            StopMovement();
            stateMachine.TransitionToIdle();
            return;
        }

        Vector3 directionToDestination = (homePosition - transform.position).normalized;
        Vector3 desiredVelocity = directionToDestination * maxSpeed;
        Vector3 avoidance = CalculateAvoidance();

        desiredVelocity += avoidance * avoidanceWeight;
        desiredVelocity = Vector3.ClampMagnitude(desiredVelocity, maxSpeed);

        acceleration = desiredVelocity - velocity;

        UpdatePosition();
    }

    public void MoveAndPauseAtDestination()
    {
        if (HasReachedDestination())
        {
            StopMovement();
            return;
        }

        Vector3 directionToDestination = (homePosition - transform.position).normalized;
        Vector3 desiredVelocity = directionToDestination * maxSpeed;
        Vector3 avoidance = CalculateAvoidance();

        desiredVelocity += avoidance * avoidanceWeight;
        desiredVelocity = Vector3.ClampMagnitude(desiredVelocity, maxSpeed);

        acceleration = desiredVelocity - velocity;

        UpdatePosition();
    }

    private void StopMovement()
    {
        velocity = Vector3.zero;
    }

    private void UpdateHomePosition()
    {
        homePosition = PlayerController.instance.transform.position + Vector3.up * 3f;
    }

    private void CalculateIdleMovement()
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
        velocity = Vector3.ClampMagnitude(velocity + acceleration * Time.deltaTime, maxSpeed);
        transform.position += velocity * Time.deltaTime;

        if (velocity != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(velocity, Vector3.up);
        }
    }

    public bool HasReachedDestination()
    {
        float distanceToDestination = Vector3.Distance(transform.position, homePosition);
        return distanceToDestination < minDistance;
    }
}
