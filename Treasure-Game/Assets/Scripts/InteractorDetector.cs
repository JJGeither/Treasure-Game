using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractorDetector : MonoBehaviour
{
    private List<Interactor> _interactableObjects = new List<Interactor>();
    public Canvas _canvas;


    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.R))
        {
            InteractWithObjects();
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

    private void InteractWithObjects()
    {
        if (_interactableObjects.Count > 0)
        {
            _interactableObjects[0].Interact();
            _interactableObjects.RemoveAt(0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        var interactable = other.GetComponent<Interactor>();

        if (interactable != null)
        {
            Debug.Log("Enter");
            _interactableObjects.Add(interactable);
        }
    }

    private void OnTriggerExit(Collider other)
    {

        var interactable = other.GetComponent<Interactor>();

        if (_interactableObjects.Contains(interactable))
        {
            _interactableObjects.Remove(interactable);
        }
    }
}
