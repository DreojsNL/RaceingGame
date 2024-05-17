using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamManager : MonoBehaviour
{
    public Camera[] cameras; // Array to hold all the cameras
    private int currentCameraIndex = 0; // Index of the currently active camera
    private bool switchButtonPressed = false; // Flag to track if the camera switch button is pressed

    // Start is called before the first frame update
    void Start()
    {
        // Disable all cameras except the first one
        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].gameObject.SetActive(i == 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check for input to switch cameras
        if (Input.GetAxis("CameraSwitch") == 1)
        {
            // If the switch button is pressed and was not pressed in the previous frame, toggle camera
            if (!switchButtonPressed)
            {
                ToggleNextCamera();
                switchButtonPressed = true; // Set the flag to true
            }
        }
        else
        {
            switchButtonPressed = false; // Reset the flag when the button is released
        }
    }

    void ToggleNextCamera()
    {
        // Disable the current camera
        cameras[currentCameraIndex].gameObject.SetActive(false);

        // Move to the next camera index, or wrap around to the beginning if at the end
        currentCameraIndex = (currentCameraIndex + 1) % cameras.Length;

        // Enable the new current camera
        cameras[currentCameraIndex].gameObject.SetActive(true);
    }
}
