using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DroneStats))]
public class PlayerDrones : MonoBehaviour
{
    public List<DroneController> followingDrones { get; private set; } = new List<DroneController>();
    public int DroneSelectionIndex = 0;

    [Header("Oxygen Variables")]
    public float OxygenMaxLevel;
    public float OxygenLevel;

    void Start()
    {
        OxygenLevel = OxygenMaxLevel;
        StartCoroutine(DecreaseOxygenLevel());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && followingDrones.Count > 0)
        {
            followingDrones[DroneSelectionIndex].abilities.PerformAction();
        }

        CycleThroughDrone();
    }

    private void CycleThroughDrone()
    {
        int maxIndex = followingDrones.Count - 1;

        maxIndex = Mathf.Max(maxIndex, 0);

        if (Input.GetKeyDown(KeyCode.L))
        {
            DroneSelectionIndex = (DroneSelectionIndex + 1) % (maxIndex + 1);
            Debug.Log("Current DroneSelectionIndex: " + DroneSelectionIndex);
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            DroneSelectionIndex = (DroneSelectionIndex - 1 + (maxIndex + 1)) % (maxIndex + 1);
            Debug.Log("Current DroneSelectionIndex: " + DroneSelectionIndex);
        }

    }

    private IEnumerator DecreaseOxygenLevel()
    {
        while (OxygenLevel > 0)
        {
            yield return new WaitForSeconds(10);
            OxygenLevel -= 1;
            Debug.Log("Oxygen Level: " + OxygenLevel);
        }

    }
}