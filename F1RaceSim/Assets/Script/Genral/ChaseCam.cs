using UnityEngine;

public class ChaseCam : MonoBehaviour
{
    public Transform target; // Reference to the car's transform
    public float distance = 5f; // Distance from the car
    public float height = 2f; // Height of the camera above the car
    public float damping = 5f; // Damping factor for smooth rotation

    // Update is called once per frame
    void LateUpdate()
    {
        if (target == null)
            return;

        // Calculate the desired position behind the car
        Vector3 desiredPosition = target.position - target.forward * distance + Vector3.up * height;

        // Smoothly move the camera towards the desired position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * damping);

        // Calculate the look direction (from camera to car) and ignore vertical component
        Vector3 lookDirection = target.position - transform.position;
        lookDirection.y = 0f;

        // Apply rotation to make the camera look at the car
        Quaternion rotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * damping);
    }
}
