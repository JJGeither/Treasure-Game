using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactee : MonoBehaviour
{
    public enum InteracteeType
    {
        Default,
        Pickup,
        Dropoff,
        DroneStartup
    }

    public InteracteeType GetInteractionType()
    {
        return interactionType;
    }

    public InteracteeType interactionType = InteracteeType.Default;

    public void Interact(Interactor interactor)
    {
        if (interactor == null)
        {
            Debug.Log("This object cannot interact");
            return;
        }

        switch (interactionType)
        {
            case InteracteeType.Default:
                interactor.interactionHandler.HandleDefaultInteraction(GetInteracteeTransform());
                break;
            case InteracteeType.Pickup:
                interactor.interactionHandler.HandlePickupInteraction(GetInteracteeTransform());
                break;
            case InteracteeType.Dropoff:
                interactor.interactionHandler.HandleDropoffInteraction(GetInteracteeTransform());
                break;
            case InteracteeType.DroneStartup:
                interactor.interactionHandler.HandleDroneStartupInteraction(GetInteracteeTransform());
                break;
            default:
                Debug.LogWarning("Unhandled interaction type: " + interactionType);
                break;
        }
    }
    public Transform GetInteracteeTransform()
    {
        return this.transform;
    }

}
