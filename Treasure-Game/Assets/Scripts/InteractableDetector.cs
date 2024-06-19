using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractorDetector : MonoBehaviour
{
    private List<Interactee> _interactableObjects = new List<Interactee>();
    public Canvas _canvas;


    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.R))
        {
            InteractWithObjects(PlayerController.instance.interactor);
        }

        if (Input.GetKeyDown(KeyCode.Q) && PlayerController.instance.followingDrones.Count > 0)
        {
           
            InteractWithObjects(PlayerController.instance.followingDrones[0].interactor);
        }

        if (Input.GetKeyDown(KeyCode.V)) {
            PlayerController.instance.followingDrones[0].MoveDroneTo(Vector3.zero);
        }

        if (_interactableObjects.Count > 0)
        {
            _canvas.enabled = true;
            _canvas.transform.position = _interactableObjects[0].transform.position;
        } else
        {
            _canvas.enabled = false;
        }
    }

    private void InteractWithObjects(Interactor interactor)
    {
        if (_interactableObjects.Count > 0)
        {
            interactor.Interact(_interactableObjects[0]);
            //_interactableObjects[0].Interact(interactor);
            _interactableObjects.RemoveAt(0);
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
}
