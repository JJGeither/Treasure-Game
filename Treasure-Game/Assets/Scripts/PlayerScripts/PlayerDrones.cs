using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DroneStatistics))]
public class PlayerDrones : MonoBehaviour
{
    public List<DroneScript> followingDrones { get; private set; } = new List<DroneScript>();

    [Header("Oxygen Variables")]
    public float OxygenMaxLevel;
    public float OxygenLevel;

    void Start()
    {
        OxygenLevel = OxygenMaxLevel;
        StartCoroutine(DecreaseOxygenLevel());
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