using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReticleController : MonoBehaviour
{
    public CameraSystem cameraSystem;
    private Transform parentTransform;
    public float baseDistanceInFront = 2.0f;
    public float movementSpeed = 0.2f;
    private float adjustedDistance = 0;
    public LayerMask groundLayer;
    public Material redMaterial;
    public Material whiteMaterial;
    private Renderer reticleRenderer;

    // Start is called before the first frame update
    void Start()
    {
        // Cache the parent's transform
        parentTransform = this.transform.parent.transform;

        // Optionally, you could add a check to ensure the parent transform is not null
        if (parentTransform == null)
        {
            Debug.LogError("Parent transform is missing.");
        }

        // Cache the reticle's renderer
        reticleRenderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (cameraSystem != null && parentTransform != null)
        {
            // Adjust distance based on the camera's Y rotation and clamp it between 0 and baseDistanceInFront
            adjustedDistance = Mathf.Clamp(adjustedDistance + Input.GetAxis("Vertical") * movementSpeed, 0, baseDistanceInFront);

            // Calculate the new forward position
            Vector3 forwardPosition = parentTransform.position + parentTransform.forward * adjustedDistance;

            // Perform a downward raycast from the forward position
            RaycastHit hit;
            if (Physics.Raycast(forwardPosition + Vector3.up * 10, Vector3.down, out hit, Mathf.Infinity, groundLayer))
            {
                // Set the reticle position to the point of first contact
                transform.position = hit.point + Vector3.up * 0.01f;
            }
            else
            {
                // If no contact, set the position to the forward position
                transform.position = forwardPosition;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        // Check if the collided object has the Interactee component
        Interactee collisionInteractee = other.gameObject.GetComponent<Interactee>();
        if (collisionInteractee)
        {
            // Set the reticle material to red
            reticleRenderer.material = redMaterial;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the collided object has the Interactee component
        Interactee collisionInteractee = other.gameObject.GetComponent<Interactee>();
        if (collisionInteractee)
        {
            // Set the reticle material back to white
            reticleRenderer.material = whiteMaterial;
        }
    }
}
