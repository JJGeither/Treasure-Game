using UnityEngine;

public class DroneStats : MonoBehaviour
{
    public string DroneName { get; private set; }
    public int MiningSpeed = 50;

    public void GenerateRandomName()
    {
        int randomNumber = Random.Range(1, 100);
        DroneName = $"Drone Buddy {randomNumber}";
        Debug.Log($"Assigned Drone Name: {DroneName}");
    }

    public class DroneStateMachine
    {
        private DroneState currentState;
        public bool IsActive { get; private set; }
        private readonly DroneController drone;

        public DroneStateMachine(DroneController droneController)
        {
            IsActive = false;
            drone = droneController;
            currentState = new DeactivatedState(drone);
        }

        public void Activate()
        {
            IsActive = true;
            PlayerController.instance.playerDrones.followingDrones.Add(drone);
            TransitionTo(new IdleState(drone));
        }

        public void TransitionTo(DroneState newState)
        {
            currentState = newState;
        }

        public void TransitionToIdle()
        {
            TransitionTo(new IdleState(drone));
        }

        public void ExecuteCurrentState()
        {
            currentState.Execute();
        }

        public bool IsBusy() => currentState.IsBusy();
    }

    public abstract class DroneState
    {
        protected readonly DroneController drone;

        protected DroneState(DroneController drone)
        {
            this.drone = drone;
        }

        public abstract void Execute();
        public abstract bool IsBusy();
    }

    public class DeactivatedState : DroneState
    {
        public DeactivatedState(DroneController drone) : base(drone) { }

        public override void Execute()
        {
            drone.Initialize();
            PlayerController.instance.playerDrones.followingDrones.Add(drone);
            drone.stateMachine.Activate();
        }

        public override bool IsBusy() => false;
    }

    public class IdleState : DroneState
    {
        public IdleState(DroneController drone) : base(drone) { }

        public override void Execute()
        {
            drone.IdleMovement();
        }

        public override bool IsBusy() => false;
    }

    public class MovementState : DroneState
    {
        public MovementState(DroneController drone, Vector3 destination) : base(drone)
        {
            drone.SetDestination(destination);
        }

        public override void Execute()
        {
            drone.MoveToDestination();
        }

        public override bool IsBusy() => true;
    }


    public class MiningState : DroneState
    {
        public MiningState(DroneController drone, Vector3 destination) : base(drone)
        {
            drone.SetDestination(destination);
        }

        public override void Execute()
        {
            drone.MoveAndPauseAtDestination();
        }

        public override bool IsBusy() => true;
    }
}

