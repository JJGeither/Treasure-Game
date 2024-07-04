using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DroneShop : MonoBehaviour
{
    public Canvas droneUpgradeScreen;
    public TMP_Dropdown droneDropdown;

    public Button miningSpeedButton;

    void Start()
    {
        droneUpgradeScreen.enabled = false;
        miningSpeedButton.onClick.AddListener(IncreaseMiningSpeed);
    }

    public void ToggleShop()
    {
        PopulateDropdown();
        if (PlayerController.instance.playerDrones.followingDrones.Count > 0)
        {
            bool isActive = droneUpgradeScreen.enabled;
            droneUpgradeScreen.enabled = !isActive;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = !isActive;
        } else
        {
            droneUpgradeScreen.enabled = false;
        }

    }

    void PopulateDropdown()
    {
        droneDropdown.ClearOptions();
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

        foreach (DroneController drone in PlayerController.instance.playerDrones.followingDrones)
        {
            options.Add(new TMP_Dropdown.OptionData(drone.GetDroneName()));
        }

        droneDropdown.AddOptions(options);
    }

    void IncreaseMiningSpeed()
    {
        DroneController selectedDrone = PlayerController.instance.playerDrones.followingDrones
                   .FirstOrDefault(drone => drone.GetDroneName() == GetSelectedDrone());
        selectedDrone.droneStats.MiningSpeed += 10;
    }

    string GetSelectedDrone()
    {
        int selectedIndex = droneDropdown.value; // Get the index of the selected option
        if (selectedIndex >= 0 && selectedIndex < droneDropdown.options.Count)
        {
            return droneDropdown.options[selectedIndex].text; // Return the text of the selected option
        }
        return null;
    }
    private void OnTriggerExit(Collider other)
    {
        if (droneUpgradeScreen.enabled)
        {
            droneUpgradeScreen.enabled = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
