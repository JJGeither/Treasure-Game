using System.Collections.Generic;
using UnityEngine;

public class Interactee : MonoBehaviour
{
    public enum InteracteeType
    {
        Default,
        Pickup,
        Dropoff,
        DroneStartup,
        OxygenShop,
        MineShop,
        DroneShop,
        Ore,
        MoveDoor,
        MoveDoorBack,
    }

    //public InteracteeType interactionType = InteracteeType.Default;

    public List<InteracteeType> interactionList;

    public InteracteeType GetInteractionType(int action = 0)
    {
        if (interactionList.Count > 0) { return interactionList[action]; }
        Debug.Log("Error: Object has no interaction");
        return 0;
    }

    public Transform GetInteracteeTransform()
    {
        return transform;
    }
}
