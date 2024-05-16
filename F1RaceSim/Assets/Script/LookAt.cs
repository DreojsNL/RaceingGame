using UnityEngine;

public class LookAt : MonoBehaviour
{
    public bool rotateOnX = true;
    public bool rotateOnY = true;
    public bool rotateOnZ = true;

    void Update()
    {
        // Find all active cameras in the scene
        Camera[] cameras = Camera.allCameras;

        if (cameras.Length > 0)
        {
            // Choose the first active camera found
            Camera mainCamera = cameras[0];

            // Get the direction from the object to the camera
            Vector3 directionToCamera = mainCamera.transform.position - transform.position;

            // Calculate the rotation to face the camera
            Quaternion lookRotation = Quaternion.LookRotation(directionToCamera);

            // If the rotation on X axis is not allowed, set X rotation to 0
            if (!rotateOnX)
            {
                lookRotation.eulerAngles = new Vector3(0, lookRotation.eulerAngles.y, lookRotation.eulerAngles.z);
            }

            // If the rotation on Y axis is not allowed, set Y rotation to 0
            if (!rotateOnY)
            {
                lookRotation.eulerAngles = new Vector3(lookRotation.eulerAngles.x, 0, lookRotation.eulerAngles.z);
            }

            // If the rotation on Z axis is not allowed, set Z rotation to 0
            if (!rotateOnZ)
            {
                lookRotation.eulerAngles = new Vector3(lookRotation.eulerAngles.x, lookRotation.eulerAngles.y, 0);
            }

            // Apply the limited rotation
            transform.rotation = lookRotation;
        }
        else
        {
            Debug.LogWarning("No active camera found in the scene.");
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Road"))
        {
            Destroy(gameObject);
        }
    }
     
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Road"))
        {
            Destroy(gameObject);
        }
    }
}
