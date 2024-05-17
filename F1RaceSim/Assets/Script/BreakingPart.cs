using UnityEngine;

public class BreakingPart : MonoBehaviour
{
    public bool broken = false; // Flag to indicate if the part is broken

    private Rigidbody rb;
    private BoxCollider collider;
    public Transform originalParent; // Store the original parent before unparenting

    void Start()
    {
        // Get the Rigidbody component attached to this GameObject
        rb = GetComponent<Rigidbody>();

        // Get the BoxCollider component attached to this GameObject
        collider = GetComponent<BoxCollider>();

        // Store the original parent
        originalParent = transform.parent;

        // Initially, set the Rigidbody to non-kinematic and disable the BoxCollider
        rb.isKinematic = true;
        collider.enabled = false;
    }

    void Update()
    {
        // Check if the part is broken
        if (broken)
        {
            // Set the Rigidbody to kinematic
            rb.isKinematic = false;

            // Enable the BoxCollider
            collider.enabled = true;

            // Unparent the object if it's currently a child of the car
            if (transform.parent == originalParent)
            {
                transform.parent = null;
            }
        }
        else
        {
            // If the part is not broken, set the Rigidbody to non-kinematic and disable the BoxCollider
            rb.isKinematic = true;
            collider.enabled = false;

            // Reparent the object if it's not already a child of the car
            if (transform.parent != originalParent)
            {
                transform.parent = originalParent;
            }
        }
    }
}
