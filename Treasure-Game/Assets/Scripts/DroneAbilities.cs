using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneAbilities : MonoBehaviour
{
    public GameObject dronePlatformPrefab;
    public GameObject bulletPrefab;
    private IDroneAbilityManager droneAbilityManager;

    void Start()
    {
        //droneAbilityManager = new NoAbility();
        //droneAbilityManager = new MakePlatform(dronePlatformPrefab, PlayerController.instance.GetComponent<Transform>());
        droneAbilityManager = new Shoot(bulletPrefab, this.transform);
    }

    public void PerformAction()
    {
        droneAbilityManager.PerformAction();
    }

    public interface IDroneAbilityManager
    {
        void PerformAction();
    }

    public class NoAbility : IDroneAbilityManager
    {
        public NoAbility() { }
        public void PerformAction() { }
    }

    public class MakePlatform : IDroneAbilityManager
    {
        private GameObject platformPrefab;
        private Transform parentTransform;

        public MakePlatform(GameObject prefab, Transform parent)
        {
            platformPrefab = prefab;
            parentTransform = parent;
        }

        public void PerformAction()
        {
            Instantiate(platformPrefab, parentTransform.position + Vector3.down * 2, parentTransform.rotation);
        }
    }

    public class Shoot : IDroneAbilityManager
    {
        private GameObject bulletPrefab;
        private Transform parentTransform;
        public Shoot(GameObject prefab, Transform parent)
        {
            bulletPrefab = prefab;
            parentTransform = parent;
        }

        public void PerformAction()
        {
            Vector3 spawnPosition = parentTransform.position + parentTransform.forward * 2f;

            // Instantiate the bullet at the parent transform's position and rotation
            GameObject bullet = GameObject.Instantiate(bulletPrefab, spawnPosition, parentTransform.rotation);

            // Get the Rigidbody component of the bullet
            Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();

            // If the bullet has a Rigidbody, add a forward force to it
            if (bulletRigidbody != null)
            {
                bulletRigidbody.AddForce(parentTransform.forward * 50f, ForceMode.Impulse);
            }
        }

    }


    public void TriggerAbility()
    {
        if (droneAbilityManager != null)
        {
            droneAbilityManager.PerformAction();
        }
    }
}
