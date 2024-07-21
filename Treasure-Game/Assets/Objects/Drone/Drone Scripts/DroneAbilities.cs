using System.Collections;
using UnityEngine;

public class DroneAbilities : MonoBehaviour
{
    public GameObject dronePlatformPrefab;
    public GameObject bulletPrefab;
    public Transform targetTransform;

    private DroneAbilityManager abilityManager;
    private DroneController droneController;

    private void Start()
    {
        droneController = GetComponent<DroneController>();
        abilityManager = new NoAbility();
    }

    public void SetAbility(DroneAbilityManager ability)
    {
        abilityManager = ability;
    }

    public void SetAbilityShoot()
    {
        abilityManager = new Shoot(bulletPrefab, droneController, PlayerController.instance.gameReticle.transform);
    }

    public void SetAbilityPlatform()
    {
        abilityManager = new MakePlatform(dronePlatformPrefab, PlayerController.instance.reticleController.transform);
    }

    public void PerformAction()
    {
        if (!abilityManager.isOnCooldown)
        {
            abilityManager.PerformAction();
            StartCoroutine(StartCooldown(abilityManager));
        }
    }

    private IEnumerator StartCooldown(DroneAbilityManager manager)
    {
        manager.isOnCooldown = true;
        yield return new WaitForSeconds(manager.cooldownDuration);
        manager.isOnCooldown = false;
    }

    public abstract class DroneAbilityManager
    {
        public abstract void PerformAction();

        public float cooldownDuration = 0f;
        public bool isOnCooldown = false;
    }

    public class NoAbility : DroneAbilityManager
    {
        public NoAbility()
        {
            cooldownDuration = 0f;
        }

        public override void PerformAction()
        {
            // No action for no ability
        }
    }

    public class MakePlatform : DroneAbilityManager
    {
        private GameObject platformPrefab;
        private Transform parentTransform;

        public MakePlatform(GameObject prefab, Transform parent)
        {
            platformPrefab = prefab;
            parentTransform = parent;
            cooldownDuration = 3f;
        }

        public override void PerformAction()
        {
            Instantiate(platformPrefab, parentTransform.position + Vector3.down * 2, parentTransform.rotation);
        }
    }

    public class Shoot : DroneAbilityManager
    {
        private GameObject bulletPrefab;
        private Transform parentTransform;
        private Transform target;
        private DroneController droneController;
        private Coroutine resetStateCoroutine;

        public Shoot(GameObject prefab, DroneController controller, Transform target)
        {
            bulletPrefab = prefab;
            droneController = controller;
            parentTransform = controller.transform;
            this.target = target;
            cooldownDuration = 0f;
        }

        public override void PerformAction()
        {
            droneController.stateMachine.TransitionTo(new DroneStats.LookAtObject(droneController, target.position));

            if (resetStateCoroutine != null)
            {
                droneController.StopCoroutine(resetStateCoroutine);
            }

            resetStateCoroutine = droneController.StartCoroutine(ResetStateAfterDelay());

            Vector3 spawnPosition = parentTransform.position + parentTransform.forward * 2f;
            GameObject bullet = Instantiate(bulletPrefab, spawnPosition, parentTransform.rotation);
            Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();

            if (bulletRigidbody != null)
            {
                bulletRigidbody.AddForce(parentTransform.forward * 50f, ForceMode.Impulse);
            }
        }

        private IEnumerator ResetStateAfterDelay()
        {
            yield return new WaitForSeconds(0.55f);
            droneController.stateMachine.TransitionToIdle();
        }
    }

    public void TriggerAbility()
    {
        if (abilityManager != null)
        {
            abilityManager.PerformAction();
        }
    }
}
