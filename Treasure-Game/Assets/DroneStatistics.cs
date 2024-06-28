using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneStatistics : MonoBehaviour
{
    public string droneName;
    public int miningSpeed = 50;

    private void Start()
    {
        droneName = SetRandomDroneName();
    }

    public string SetRandomDroneName()
    {
        int randomNumber = Random.Range(1, 100); // Generate a random number between 1 and 100
        string DroneName = "Drone Buddy " + randomNumber;
        Debug.Log("Assigned Drone Name: " + DroneName); // Log the assigned name for debugging purposes
        return DroneName;
    }
}
