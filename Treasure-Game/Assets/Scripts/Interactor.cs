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
                interactionHandler.HandleDefaultInteraction(interactee.transform);
                break;
            case Interactee.InteracteeType.Pickup:
                interactionHandler.HandlePickupInteraction(interactee.transform);
                break;
            case Interactee.InteracteeType.Dropoff:
                interactionHandler.HandleDropoffInteraction(interactee.transform);
                break;
            case Interactee.InteracteeType.DroneStartup:
                interactionHandler.HandleDroneStartupInteraction(interactee.transform);
                break;
            default:
                Debug.LogWarning("Unhandled interaction type: " + interactionType);
                break;
        }
    }

    private void Start()
    {
        interactionHandler = CreateInteractionHandler();
    }

    private IInteractionHandler CreateInteractionHandler()
    {
        switch (interactorType)
        {
            case InteractorType.Player:
                return new PlayerInteractionHandler(this.transform);
            case InteractorType.Drone:
                return new DroneInteractionHandler(this.transform, this.GetComponent<DroneScript>());
            case InteractorType.Default:
            default:
                return new DefaultInteractionHandler(this.transform);
        }
    }

    public abstract class IInteractionHandler
    {
        protected Transform interactorTransform;

        protected IInteractionHandler(Transform interactorTransform)
        {
            this.interactorTransform = interactorTransform;
        }

        public virtual void HandleDefaultInteraction(Transform interactee) { }
        public virtual void HandlePickupInteraction(Transform interactee) { }
        public virtual void HandleDropoffInteraction(Transform interactee) { }
        public virtual void HandleDroneStartupInteraction(Transform interactee) { }
    }

    public class DefaultInteractionHandler : IInteractionHandler
    {
        public DefaultInteractionHandler(Transform interactorTransform) : base(interactorTransform) { }

        public override void HandleDefaultInteraction(Transform interactee)
        {
            Debug.Log("Default interaction handled");
        }

        public override void HandlePickupInteraction(Transform interactee)
        {
            interactee.SetParent(interactorTransform);
            var collider = interactee.GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = false;
            }
            interactee.localPosition = Vector3.up;
            heldObject = interactee;
            Debug.Log("Picked up");
        }

        public override void HandleDropoffInteraction(Transform interactee)
        {
            Debug.Log("Dropped off");
            if (heldObject != null)
            {
                heldObject.localPosition = Vector3.zero;
                Destroy(heldObject.GetComponent<Interactor>());
                heldObject.SetParent(interactee.transform);
                heldObject = null;
                PlayerController.instance.moneyAmount += 10;
            }
        }

        public override void HandleDroneStartupInteraction(Transform interactee)
        {
            var droneScript = interactee.GetComponent<DroneScript>();
            if (droneScript != null)
            {
                droneScript.DroneStartup();
                PlayerController.instance.followingDrones.Add(droneScript);
            }
            Debug.Log("Drone started up");
        }
    }

    public class PlayerInteractionHandler : DefaultInteractionHandler
    {
        public PlayerInteractionHandler(Transform interactorTransform) : base(interactorTransform) { }
    }

    public class DroneInteractionHandler : DefaultInteractionHandler
    {
        DroneScript droneScript;
        public DroneInteractionHandler(Transform interactorTransform, DroneScript droneScript) : base(interactorTransform) { this.droneScript = droneScript;  }

        public override void HandleDefaultInteraction(Transform interactee)
        {
            Debug.Log("Default interaction handled");
        }

        public override void HandlePickupInteraction(Transform interactee)
        {
            if (droneScript != null)
            {
                droneScript.MoveDroneTo(interactee.position);
                HandleDroneReachedHome();
            }

            interactee.SetParent(interactorTransform);
            var collider = interactee.GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = false;
            }
            interactee.localPosition = Vector3.up;
            heldObject = interactee;
            Debug.Log("Picked up");
        }

        public override void HandleDropoffInteraction(Transform interactee)
        {
            Debug.Log("Dropped off");
            if (heldObject != null)
            {
                heldObject.localPosition = Vector3.zero;
                Destroy(heldObject.GetComponent<Interactor>());
                heldObject.SetParent(interactee.transform);
                heldObject = null;
                PlayerController.instance.moneyAmount += 10;
            }
        }

        public override void HandleDroneStartupInteraction(Transform interactee)
        {
            var droneScript = interactee.GetComponent<DroneScript>();
            if (droneScript != null)
            {
                droneScript.DroneStartup();
                PlayerController.instance.followingDrones.Add(droneScript);
            }
            Debug.Log("Drone started up");
        }
    }
}
