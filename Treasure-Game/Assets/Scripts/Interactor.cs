using UnityEngine;
using System.Collections;

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

    private void Start()
    {
        interactionHandler = CreateInteractionHandler();
    }

    public void Interact(Interactee interactee)
    {
        interactionHandler.HandleInteraction(interactee);
    }

    private IInteractionHandler CreateInteractionHandler()
    {
        switch (interactorType)
        {
            case InteractorType.Player:
                return new PlayerInteractionHandler(transform);
            case InteractorType.Drone:
                return new DroneInteractionHandler(transform, GetComponent<DroneScript>());
            case InteractorType.Default:
            default:
                return new DefaultInteractionHandler(transform);
        }
    }

    public abstract class IInteractionHandler
    {
        protected Transform interactorTransform;

        protected IInteractionHandler(Transform interactorTransform)
        {
            this.interactorTransform = interactorTransform;
        }

        public virtual void HandleInteraction(Interactee interactee) { }
    }

    public class DefaultInteractionHandler : IInteractionHandler
    {
        public DefaultInteractionHandler(Transform interactorTransform) : base(interactorTransform) { }

        public override void HandleInteraction(Interactee interactee)
        {
            var interactionType = interactee.GetInteractionType();

            switch (interactionType)
            {
                case Interactee.InteracteeType.Default:
                    HandleDefaultInteraction(interactee.transform);
                    break;
                case Interactee.InteracteeType.Pickup:
                    HandlePickupInteraction(interactee.transform);
                    break;
                case Interactee.InteracteeType.Dropoff:
                    HandleDropoffInteraction(interactee.transform);
                    break;
                case Interactee.InteracteeType.DroneStartup:
                    HandleDroneStartupInteraction(interactee.transform);
                    break;
                default:
                    Debug.LogWarning("Unhandled interaction type: " + interactionType);
                    break;
            }
        }

        protected virtual void HandleDefaultInteraction(Transform interactee)
        {
            Debug.Log("Default interaction handled");
        }

        protected virtual void HandlePickupInteraction(Transform interactee)
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

        protected virtual void HandleDropoffInteraction(Transform interactee)
        {
            Debug.Log("Dropped off");
            if (heldObject != null)
            {
                heldObject.localPosition = Vector3.zero;
                Destroy(heldObject.GetComponent<Interactor>());
                heldObject.SetParent(interactee.transform);
                heldObject = null;
                PlayerController.instance.playerStatistics.moneyAmount += 10;
            }
        }

        protected virtual void HandleDroneStartupInteraction(Transform interactee)
        {
            var droneScript = interactee.GetComponent<DroneScript>();
            if (droneScript != null)
            {
                droneScript.DroneStartup();
                PlayerController.instance.playerDrones.followingDrones.Add(droneScript);
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
        private readonly DroneScript droneScript;

        public DroneInteractionHandler(Transform interactorTransform, DroneScript droneScript) : base(interactorTransform)
        {
            this.droneScript = droneScript;
        }

        protected override void HandlePickupInteraction(Transform interactee)
        {
            if (droneScript != null)
            {
                MonoBehaviour monoBehavior = interactorTransform.GetComponent<MonoBehaviour>();
                monoBehavior.StartCoroutine(WaitForDroneToReachHome(interactee));
            }
        }

        private IEnumerator WaitForDroneToReachHome(Transform interactee)
        {
            droneScript.MoveDroneTo(interactee.position);

            yield return new WaitUntil(() => !droneScript.isBusy);

            base.HandlePickupInteraction(interactee);
        }
    }
}
