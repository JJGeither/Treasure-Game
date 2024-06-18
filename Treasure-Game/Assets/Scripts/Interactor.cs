using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{


    public static Transform heldObject;
    public InteractorType interactorType;
    public IInteractionHandler interactionHandler;

    public enum InteractorType
    {
        Default,
        Player,
        Drone
    }

    public void Interact(Interactee interactee)
    {
        var interactionType = interactee.GetInteractionType();

        switch (interactionType)
        {
            case Interactee.InteracteeType.Default:
                //interactor.interactionHandler.HandleDefaultInteraction(GetInteracteeTransform());
                break;
            case Interactee.InteracteeType.Pickup:
                interactionHandler.HandlePickupInteraction(interactee.transform);
                //interactor.interactionHandler.HandlePickupInteraction(GetInteracteeTransform());
                break;
            case Interactee.InteracteeType.Dropoff:
                //interactor.interactionHandler.HandleDropoffInteraction(GetInteracteeTransform());
                break;
            case Interactee.InteracteeType.DroneStartup:
                interactionHandler.HandleDroneStartupInteraction(interactee.transform);
                //interactor.interactionHandler.HandleDroneStartupInteraction(GetInteracteeTransform());
                break;
            default:
                Debug.LogWarning("Unhandled interaction type: " + interactionType);
                break;
        }
    }

    private void Start()
    {
        // Set interaction handler based on interactorType
        switch (interactorType)
        {
            case InteractorType.Default:
                interactionHandler = new DefaultInteractionHandler(this.transform);
                break;
            case InteractorType.Player:
                interactionHandler = new PlayerInteractionHandler(this.transform);
                break;
            case InteractorType.Drone:
                interactionHandler = new DroneInteractionHandler(this.transform);
                break;
            default:
                interactionHandler = new DefaultInteractionHandler(this.transform);
                break;
        }
    }

    public class IInteractionHandler
    {
        public IInteractionHandler(Transform interactorTransform)
        {
            this.interactorTransform = interactorTransform;
        }

        public void HandleDefaultInteraction(Transform interactee) { }
        public virtual void HandlePickupInteraction(Transform interactee) { }
        public void HandleDropoffInteraction(Transform interactee) { }
        public virtual void HandleDroneStartupInteraction(Transform interactee) { }

        public Transform interactorTransform;
    }

    public class DefaultInteractionHandler : IInteractionHandler
    {
        public DefaultInteractionHandler(Transform interactorTransform) : base(interactorTransform) { }
        public void HandleDefaultInteraction(Transform interactee)
        {
            // Custom logic for default interaction
            Debug.Log("Default interaction handled");
        }

        public override void HandlePickupInteraction(Transform interactee)
        {
            interactee.SetParent(interactorTransform);
            heldObject = interactee;
            Debug.Log("Picked up");
        }

        public void HandleDropoffInteraction(Transform interactee)
        {
            // Custom logic for dropoff interaction
            Debug.Log("Dropped off");
            if (heldObject != null)
            {
                Destroy(heldObject.GetComponent<Interactor>());
                heldObject.SetParent(interactee.transform);
                heldObject = null;
                PlayerController.instance.moneyAmount += 10;
            }
        }

        public override void HandleDroneStartupInteraction(Transform interactee)
        {
            // Custom logic for drone startup interaction
            var droneScript = interactee.GetComponent<DroneScript>();
            if (droneScript != null)
            {
                droneScript.DroneStartup();
                PlayerController.instance.followingDrones.Add(droneScript);
            }
            Debug.Log("Drone started up");
        }
    }

    public class PlayerInteractionHandler : IInteractionHandler
    {
        public PlayerInteractionHandler(Transform interactorTransform) : base(interactorTransform) { }
    }
    public class DroneInteractionHandler : IInteractionHandler
    {
        public DroneInteractionHandler(Transform interactorTransform) : base(interactorTransform) { }
        public void HandleDefaultInteraction(Transform interactee)
        {
            // Custom logic for default interaction
            Debug.Log("Default interaction handled");
        }

        public void HandlePickupInteraction(Transform interactee)
        {
            interactee.SetParent(PlayerController.instance.transform);
            heldObject = interactee;
            Debug.Log("Picked up");
        }

        public void HandleDropoffInteraction(Transform interactee)
        {
            // Custom logic for dropoff interaction
            Debug.Log("Dropped off");
            if (heldObject != null)
            {
                Destroy(heldObject.GetComponent<Interactor>());
                heldObject.SetParent(interactee.transform);
                heldObject = null;
                PlayerController.instance.moneyAmount += 10;
            }
        }

        public void HandleDroneStartupInteraction(Transform interactee)
        {
            // Custom logic for drone startup interaction
            var droneScript = interactee.GetComponent<DroneScript>();
            if (droneScript != null)
            {
                droneScript.DroneStartup();
                PlayerController.instance.followingDrones.Add(droneScript);
            }
            Debug.Log("Drone started up");
        }
    }


    public void HandleDefaultInteraction(Transform interactee)
    {
        Debug.Log("Interacted");
        interactionHandler.HandleDefaultInteraction(interactee);
    }

    public void HandlePickupInteraction(Transform interactee)
    {
        interactionHandler.HandlePickupInteraction(interactee);
    }

    public void HandleDropoffInteraction(Transform interactee)
    {

    }

    public void HandleDroneStartupInteraction(Transform interactee)
    {
        var droneScript = GetComponent<DroneScript>();
        if (droneScript)
        {
            droneScript.DroneStartup();
            PlayerController.instance.followingDrones.Add(droneScript);
        }
    }
}
