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

    public InteracteeType interactionType = InteracteeType.Default;

    public InteracteeType GetInteractionType()
    {
        return interactionType;
    }

    public void Interact(Interactor interactor)
    {
        if (interactor == null || interactor.interactionHandler == null)
        {
            Debug.Log("This object cannot interact");
            return;
        }

        interactor.interactionHandler.HandleInteraction(this);
    }

    public Transform GetInteracteeTransform()
    {
        return transform;
    }
}
