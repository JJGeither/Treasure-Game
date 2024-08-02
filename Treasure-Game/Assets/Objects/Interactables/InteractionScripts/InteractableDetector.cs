using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InteractorDetector : MonoBehaviour
{
    public enum InteractorType
    {
        Player,
        Drone
    }

    private List<Interactee> _interactableObjects = new List<Interactee>();
    public InteractorType interactorType;
    public Canvas _canvas;
    public Color gizmoColor = Color.green;
    public float gizmoRadius = 1.0f;

    void Update()
    {
        if (interactorType == InteractorType.Player && Input.GetKeyDown(KeyCode.R))
        {
            InteractUsingObject(PlayerController.instance.interactor);
        }

        if (interactorType == InteractorType.Drone && Input.GetKeyDown(KeyCode.Q) && PlayerController.instance.playerDrones.followingDrones.Count > 0)
        {
            InteractUsingObject(PlayerController.instance.playerDrones.followingDrones[0].interactor);
        }

        if (_interactableObjects.Count > 0)
        {
            _canvas.enabled = true;
            _canvas.transform.position = _interactableObjects[0].transform.position;
        }
        else
        {
            _canvas.enabled = false;
        }
    }

    private void InteractUsingObject(Interactor interactor)
    {
        if (_interactableObjects.Count > 0)
        {
            interactor.Interact(_interactableObjects[0], 0);
            _interactableObjects.RemoveAt(0);
        } else if (interactorType == InteractorType.Drone)
        {
            var pauseState = new DroneStats.PauseState(PlayerController.instance.playerDrones.followingDrones[0], this.transform.position);
            PlayerController.instance.playerDrones.followingDrones[0].stateMachine.TransitionTo(pauseState);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var interactable = other.GetComponent<Interactee>();
        if (interactable != null)
        {
            Debug.Log("Enter");
            _interactableObjects.Add(interactable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var interactable = other.GetComponent<Interactee>();
        if (_interactableObjects.Contains(interactable))
        {
            _interactableObjects.Remove(interactable);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, gizmoRadius);
    }
}
