using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReticleController : MonoBehaviour
{
    public Camera cameraSystem;
    public float rayDistance = 10.0f;
    public LayerMask layersToIgnore;
    public GameObject raycastInteractableDetector;

    void Start()
    {

        if (cameraSystem == null)
        {
            Debug.LogError("Camera system is missing.");
            cameraSystem = Camera.main;
        }
    }

    void Update()
    {
        if (cameraSystem != null)
        {
            RaycastHit hit;
            Vector3 endPosition = cameraSystem.transform.position + cameraSystem.transform.forward * rayDistance;

            if (Physics.Raycast(cameraSystem.transform.position, cameraSystem.transform.forward, out hit, rayDistance, ~layersToIgnore))
            {
                raycastInteractableDetector.transform.position = hit.point;
            }
            else
            {
                raycastInteractableDetector.transform.position = endPosition;
            }
        }
    }

    void OnDrawGizmos()
    {
        if (cameraSystem != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(cameraSystem.transform.position, cameraSystem.transform.forward * rayDistance);
        }
    }
}
