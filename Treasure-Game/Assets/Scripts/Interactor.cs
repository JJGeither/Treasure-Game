using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    public enum InteractionType
    {
        Default,
        Pickup,
        Dropoff,
        DroneStartup
    }

    public InteractionType interactionType = InteractionType.Default;

    public void Interact()
    {
        switch (interactionType)
        {
            case InteractionType.Default:
                Debug.Log("Interacted");
                break;
            case InteractionType.Pickup:
                transform.SetParent(PlayerController.instance.transform);
                PlayerController.instance.heldObject = transform;
                Debug.Log("Picked up");
                break;
            case InteractionType.Dropoff:
                if (PlayerController.instance.heldObject != null)
                {
                    PlayerController.instance.heldObject.SetParent(this.transform);
                    PlayerController.instance.heldObject = null;
                }
                break;
            case InteractionType.DroneStartup:
                var droneScript = this.GetComponent<DroneScript>();
                if (droneScript)
                {
                    droneScript.DroneStartup();
                }
                break;
            default:
                Debug.LogWarning("Unhandled interaction type: " + interactionType);
                break;
        }
    }
}
