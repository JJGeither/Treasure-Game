using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DroneStats))]
public class PlayerDrones : MonoBehaviour
{
    public List<DroneController> followingDrones { get; private set; } = new List<DroneController>();

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
            foreach (DroneController drone in followingDrones) { drone.abilities.PerformAction(); }
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