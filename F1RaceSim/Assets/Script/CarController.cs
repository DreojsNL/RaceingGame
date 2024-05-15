using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public WheelCollider frontLeft;
    public WheelCollider frontRight;
    public WheelCollider backLeft;
    public WheelCollider backRight;
    public Transform frontLeftM;
    public Transform frontRightM;
    public Transform backLeftM;
    public Transform backRightM;

    public float acceleration = 500f;
    public float brakeForce = 300f;
    public float maxTurnAngle = 15;

    // RPM related variables
    public float maxRPM = 6000f; // Maximum RPM of the engine
    private float currentRPM = 0f;
    private float gearChangeRPM = 2500f; // RPM threshold for gear change
    private int currentGear = 1; // Current gear
    private float[] gearRatios = { 3.5f, 2.5f, 1.8f, 1.4f }; // Gear ratios for each gear

    private float currentAcceleration = 0f;
    private float currentBrakeForce = 0f;
    private float currentTurnAngle = 0f;

    private void FixedUpdate()
    {
        // Control steering
        currentTurnAngle = maxTurnAngle * Input.GetAxis("Horizontal");
        frontRight.steerAngle = currentTurnAngle;
        frontLeft.steerAngle = currentTurnAngle;

        // Calculate RPM
        float wheelRPM = (backLeft.rpm + backRight.rpm) / 2f;
        currentRPM = Mathf.Abs(wheelRPM) * gearRatios[currentGear - 1]; // Adjust RPM based on gear ratio

        // Control acceleration based on RPM
        currentAcceleration = acceleration * Input.GetAxis("Vertical");
        if (currentRPM < maxRPM && currentGear != 0)
        {
            backLeft.motorTorque = currentAcceleration;
            backRight.motorTorque = currentAcceleration;
        }
        else
        {
            backLeft.motorTorque = 0f;
            backRight.motorTorque = 0f;
        }

        // Control braking
        if (Input.GetKey(KeyCode.Space))
        {
            currentBrakeForce = brakeForce;
        }
        else
        {
            currentBrakeForce = 0f;
        }
        frontRight.brakeTorque = currentBrakeForce;
        frontLeft.brakeTorque = currentBrakeForce;
        backLeft.brakeTorque = currentBrakeForce;
        backRight.brakeTorque = currentBrakeForce;

        // Update wheel positions and rotations
        UpdateWheel(frontLeft, frontLeftM);
        UpdateWheel(frontRight, frontRightM);
        UpdateWheel(backLeft, backLeftM);
        UpdateWheel(backRight, backRightM);

        // Handle gear shifting
        if (Input.GetKeyDown(KeyCode.LeftShift) && currentGear < gearRatios.Length) // Shift up
        {
            currentGear++;
        }
        else if (Input.GetKeyDown(KeyCode.LeftControl) && currentGear > 1) // Shift down
        {
            currentGear--;
        }
    }

    void UpdateWheel(WheelCollider col, Transform trans)
    {
        Vector3 position;
        Quaternion rotation;
        col.GetWorldPose(out position, out rotation);

        trans.position = position;
        trans.rotation = rotation;
    }
}
