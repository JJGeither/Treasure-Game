using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class DroneAbilities : MonoBehaviour
{
    public GameObject dronePlatformPrefab;
    public GameObject bulletPrefab;
    public Transform targetTransform;
    private IDroneAbilityManager droneAbilityManager;
    private DroneController droneController;

    void Start()
    {
        droneController = this.GetComponent<DroneController>();
        droneAbilityManager = new NoAbility();
        //droneAbilityManager = new MakePlatform(dronePlatformPrefab, PlayerController.instance.GetComponent<Transform>());
        //droneAbilityManager = new Shoot(bulletPrefab, droneController, targetTransform);
    }

    public void SetAbility(IDroneAbilityManager ability)
    {
        droneAbilityManager = ability;
    }

    public void SetAbilityShoot()
    {
        droneAbilityManager = new Shoot(bulletPrefab, droneController, targetTransform);
    }

    public void SetAbilityPlatform()
    {
        droneAbilityManager = new MakePlatform(dronePlatformPrefab, PlayerController.instance.GetComponent<Transform>());
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
        private Transform target;
        private DroneController droneController;
        private Coroutine resetStateCoroutine;

        public Shoot(GameObject prefab, DroneController droneController, Transform target)
        {
            bulletPrefab = prefab;
            parentTransform = droneController.GetComponent<Transform>();
            this.target = target;
            this.droneController = droneController;
        }

        public void PerformAction()
        {
            // Transition to LookAtObject state
            droneController.stateMachine.TransitionTo(new DroneStats.LookAtObject(droneController, target.position));

            // Cancel any existing reset coroutine
            if (resetStateCoroutine != null)
            {
                droneController.StopCoroutine(resetStateCoroutine);
            }

            // Start a new reset coroutine
            resetStateCoroutine = droneController.StartCoroutine(ResetStateAfterDelay());

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

        private IEnumerator ResetStateAfterDelay()
        {
            // Wait for 2 seconds
            yield return new WaitForSeconds(.55f);

            // Transition to Idle state
            droneController.stateMachine.TransitionToIdle();
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
