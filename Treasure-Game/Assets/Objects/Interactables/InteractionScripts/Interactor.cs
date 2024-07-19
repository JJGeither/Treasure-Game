using UnityEngine;
using System.Collections;

public class Interactor : MonoBehaviour
{
    public static Transform heldObject;
    public InteractorType interactorType;
    private IInteractionHandler interactionHandler;

    public enum InteractorType
    {
        Default,
        Player,
        Drone,
    }

    private void Start()
    {
        interactionHandler = CreateInteractionHandler();
    }

    public void Interact(Interactee interactee, int action)
    {
        if (interactee == null)
        {
            Debug.Log("This object cannot interact");
            return;
        }

        interactionHandler.HandleInteraction(interactee, action);
    }

    private IInteractionHandler CreateInteractionHandler()
    {
        switch (interactorType)
        {
            case InteractorType.Player:
                return new PlayerInteractionHandler(transform);
            case InteractorType.Drone:
                return new DroneInteractionHandler(transform, GetComponent<DroneController>());
            default:
                return new DefaultInteractionHandler(transform);
        }
    }

    public abstract class IInteractionHandler
    {
        protected Transform interactorTransform;

        protected IInteractionHandler(Transform interactorTransform)
        {
            this.interactorTransform = interactorTransform;
        }

        public abstract void HandleInteraction(Interactee interactee, int action);
    }

    public class DefaultInteractionHandler : IInteractionHandler
    {
        public DefaultInteractionHandler(Transform interactorTransform) : base(interactorTransform) { }

        public override void HandleInteraction(Interactee interactee, int action)
        {
            switch (interactee.GetInteractionType(action))
            {
                case Interactee.InteracteeType.Default:
                    HandleDefaultInteraction(interactee.transform);
                    break;
                case Interactee.InteracteeType.Pickup:
                    HandlePickupInteraction(interactee.transform);
                    break;
                case Interactee.InteracteeType.Dropoff:
                    HandleDropoffInteraction(interactee.transform);
                    break;
                case Interactee.InteracteeType.DroneStartup:
                    HandleDroneStartupInteraction(interactee.transform);
                    break;
                case Interactee.InteracteeType.OxygenShop:
                    HandleOxygenShopInteraction();
                    break;
                case Interactee.InteracteeType.MineShop:
                    HandleMineShopInteraction();
                    break;
                case Interactee.InteracteeType.DroneShop:
                    HandleDroneShopInteraction(interactee);
                    break;
                case Interactee.InteracteeType.Ore:
                    HandleOreInteraction(interactee.transform);
                    break;
                case Interactee.InteracteeType.MoveDoor:
                    HandleDoorOpening(interactee.transform);
                    break;
                case Interactee.InteracteeType.MoveDoorBack:
                    HandleDoorClosing(interactee.transform);
                    break;
                default:
                    Debug.LogWarning("Unhandled interaction type: " + interactee.GetInteractionType(action));
                    break;
            }
        }

        protected virtual void HandleDoorOpening(Transform interactee)
        {
            interactee.position += new Vector3(10,0,0);
        }

        protected virtual void HandleDoorClosing(Transform interactee)
        {
            interactee.position -= new Vector3(10, 0, 0);
        }

        protected virtual void HandleDefaultInteraction(Transform interactee)
        {
            Debug.Log("Default interaction handled");
        }

        protected virtual void HandlePickupInteraction(Transform interactee)
        {
            SetInteracteeAsChild(interactee);
            DisableCollider(interactee);
            SetHeldObject(interactee);
            Debug.Log("Picked up");
        }

        protected virtual void HandleDroneShopInteraction(Interactee interactee)
        {
            DroneShop droneShop = interactee.GetComponent<DroneShop>();
            droneShop.ToggleShop();
        }

        private void SetInteracteeAsChild(Transform interactee)
        {
            interactee.SetParent(interactorTransform);
            interactee.localPosition = Vector3.up;
        }

        private void DisableCollider(Transform interactee)
        {
            var collider = interactee.GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = false;
            }
        }

        private void SetHeldObject(Transform interactee)
        {
            heldObject = interactee;
        }

        protected virtual void HandleDropoffInteraction(Transform interactee)
        {
            Debug.Log("Dropped off");
            if (heldObject != null)
            {
                ResetHeldObjectPosition();
                Destroy(heldObject.GetComponent<Interactor>());
                heldObject.SetParent(interactee);
                heldObject = null;
                PlayerController.instance.playerStatistics.moneyAmount += 10;
            }
        }

        private void ResetHeldObjectPosition()
        {
            heldObject.localPosition = Vector3.zero;
        }

        protected virtual void HandleDroneStartupInteraction(Transform interactee)
        {
            var droneController = interactee.GetComponent<DroneController>();
            if (droneController != null)
            {
                droneController.stateMachine.Activate();
            }
            Debug.Log("Drone started up");
        }

        protected virtual void HandleOxygenShopInteraction()
        {
            if (PlayerController.instance.playerStatistics.moneyAmount >= 10)
            {
                PlayerController.instance.playerStatistics.moneyAmount -= 10;
                PlayerController.instance.playerDrones.OxygenMaxLevel += 10;
                PlayerController.instance.playerDrones.OxygenLevel += 10;
            }
        }

        protected virtual void HandleMineShopInteraction()
        {
            if (PlayerController.instance.playerStatistics.moneyAmount >= 10)
            {
                PlayerController.instance.playerStatistics.moneyAmount -= 10;
                PlayerController.instance.playerStatistics.playerMineLevel += 10;
            }
        }

        protected virtual void HandleOreInteraction(Transform interactee) { }
    }

    public class PlayerInteractionHandler : DefaultInteractionHandler
    {
        public PlayerInteractionHandler(Transform interactorTransform) : base(interactorTransform) { }

        protected override void HandleOreInteraction(Transform interactee)
        {
            MonoBehaviour monoBehaviour = interactorTransform.GetComponent<MonoBehaviour>();
            OreController oreController = interactee.GetComponent<OreController>();
            int mineLevel = oreController.mineLevel;
            monoBehaviour.StartCoroutine(MineOre(interactee.transform));
        }

        private IEnumerator MineOre(Transform interactee)
        {
            var oreScript = interactee.GetComponent<OreController>();
            if (oreScript != null && oreScript.oreDurability <= PlayerController.instance.playerStatistics.playerMineLevel)
            {
                while (oreScript.oreDurability > 0)
                {
                    Debug.Log(oreScript.oreDurability);
                    oreScript.oreDurability -= 5;
                    PlayerController.instance.playerStatistics.moneyAmount += 10;
                    yield return new WaitForSeconds(1f); // Wait for 1 second before the next level decrease
                }
                Destroy(interactee.gameObject); // Destroy the ore once mine level reaches 0
            }
        }
    }

    public class DroneInteractionHandler : DefaultInteractionHandler
    {
        private readonly DroneController droneController;

        public DroneInteractionHandler(Transform interactorTransform, DroneController droneScript) : base(interactorTransform)
        {
            this.droneController = droneScript;
        }

        protected override void HandleOreInteraction(Transform interactee)
        {
            MonoBehaviour monoBehaviour = interactorTransform.GetComponent<MonoBehaviour>();
            droneController.stateMachine.TransitionTo(new DroneStats.MiningState(droneController, interactee.position));
            monoBehaviour.StartCoroutine(MineOre(interactee.transform, droneController.stateMachine));
        }

        private IEnumerator MineOre(Transform interactee, DroneStats.DroneStateMachine stateMachine)
        {
            droneController.SetDestination(interactee.position);
            var oreScript = interactee.GetComponent<OreController>();
            if (oreScript != null && oreScript.oreDurability <= PlayerController.instance.playerStatistics.playerMineLevel)
            {
                while (oreScript.oreDurability > 0)
                {
                    Debug.Log(oreScript.oreDurability);
                    oreScript.oreDurability -= 10;
                    PlayerController.instance.playerStatistics.moneyAmount += 10;
                    yield return new WaitForSeconds(1f); // Wait for 1 second before the next level decrease
                }
                Destroy(interactee.gameObject); // Destroy the ore once mine level reaches 0
                stateMachine.TransitionToIdle();
            }
        }

        protected override void HandlePickupInteraction(Transform interactee)
        {
            if (droneController != null)
            {
                droneController.stateMachine.TransitionTo(new DroneStats.MovementState(droneController, interactee.position));
                MonoBehaviour monoBehaviour = interactorTransform.GetComponent<MonoBehaviour>();
                monoBehaviour.StartCoroutine(DronePickupWhenReachHome(interactee));
            }
        }

        private IEnumerator DronePickupWhenReachHome(Transform interactee)
        {;
            yield return new WaitUntil(() => !droneController.stateMachine.IsBusy());
            base.HandlePickupInteraction(interactee);
        }

        protected override void HandleDropoffInteraction(Transform interactee)
        {
            if (droneController != null)
            {
                droneController.stateMachine.TransitionTo(new DroneStats.MovementState(droneController, interactee.position));
                MonoBehaviour monoBehaviour = interactorTransform.GetComponent<MonoBehaviour>();
                monoBehaviour.StartCoroutine(DroneDropoffWhenReachHome(interactee));
            }
        }

        private IEnumerator DroneDropoffWhenReachHome(Transform interactee)
        {
            yield return new WaitUntil(() => !droneController.stateMachine.IsBusy());
            base.HandleDropoffInteraction(interactee);
        }
    }
}
