using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using UnityEngine.EventSystems;

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
    public TextMeshProUGUI rmpText;
    public TextMeshProUGUI kmhText;
    public TextMeshProUGUI gearText;
    public TextMeshProUGUI engineStatus;
    public AnimationCurve hpToRPMCurve;
    private string currentRPMstring;

    public float acceleration = 500f;
    public float brakeForce = 300f;
    public float maxTurnAngle = 15;

    // RPM related variables
    public float maxRPM = 6000f; // Maximum RPM of the engine
    public float idleRPM = 1000f;
    private bool engineIsBroke;
    private bool engineOn;
    private float currentRPM = 0f;
    public int currentGear = 1; // Current gear
    public float[] gearRatios = { 3.5f, 2.5f, 1.8f, 1.4f };
    public float differentialRatio;

    public float clutch;
    private float clutchInvers;
    private float gasInput;
    private float currentAcceleration = 0f;
    private float currentBrakeForce = 0f;
    private float currentTurnAngle = 0f;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        if (engineOn)
        {
            engineStatus.text = "Engine Status:on";
        }
        else
        {
            engineStatus.text = "Engine Status:off";
        }
        if (engineIsBroke)
        {
            engineStatus.text = "Engine Status:broken";
        }
        if (Input.GetKeyDown(KeyCode.I)) // startCar
        {
        engineOn = !engineOn;
        }
        if (!engineOn)
        {
            currentRPM = 0f;
        }
        gearText.text = currentGear.ToString();
        clutchInvers = Input.GetAxis("Clutch");
        clutch = 1f - clutchInvers;
        kmhText.text = "KMH:" + (rb.velocity.magnitude * 3.6f).ToString(format:"000");
        if (Input.GetKeyDown(KeyCode.LeftShift) && clutch == 2) // Shift up
        {
            currentGear++;
            if(!engineIsBroke)
            {
                currentRPM = currentRPM - 5000;
            }
            
        }
        else if (Input.GetKeyDown(KeyCode.LeftControl) && currentGear > 1 && clutch == 2) // Shift down
        {
            currentGear--;
            if (!engineIsBroke)
            {
                currentRPM = currentRPM + 5000;
            }
        }
    }
    private void FixedUpdate()
    {
        currentRPMstring = currentRPM.ToString(format: "0000");
        rmpText.text = "RPM:" + currentRPMstring;
        // Control steering
        currentTurnAngle = maxTurnAngle * Input.GetAxis("Horizontal");
        frontRight.steerAngle = currentTurnAngle;
        frontLeft.steerAngle = currentTurnAngle;

        if (!engineIsBroke || !engineOn)
        {
        gasInput = Input.GetAxis("Vertical");
        if(clutch == 1)
        {
            int random = Random.Range(-50, 50);
            float wheelRPM = Mathf.Abs((backRight.rpm + backLeft.rpm) / 2f) * gearRatios[currentGear] * differentialRatio;
            currentRPM = Mathf.Lerp(currentRPM, Mathf.Max(idleRPM - 100, wheelRPM), Time.deltaTime * 3f);
            currentAcceleration = (hpToRPMCurve.Evaluate(currentRPM / maxRPM) * acceleration / currentRPM) * gearRatios[currentGear] * differentialRatio * 5252f * clutch;
        }
        else
        {
            int random = Random.Range(-50, 50);
            currentRPM = Mathf.Lerp(currentRPM, Mathf.Max(idleRPM, maxRPM * gasInput) + random, Time.deltaTime);
            currentAcceleration = 0f;
        }
        if (currentRPM < maxRPM && currentGear != 0 && engineOn)
        {
            backLeft.motorTorque = currentAcceleration * gasInput;
            backRight.motorTorque = currentAcceleration * gasInput;
        }
        else
        {
            backLeft.motorTorque = 0f;
            backRight.motorTorque = 0f;
        }
        if(gasInput == -1 )
        {
            currentBrakeForce = brakeForce;
        }
        if(currentRPM < 1000 && clutch == 1)
            {
                EngineStall();
            }
        if(currentRPM > 10000)
        {
            currentBrakeForce = 3000;
            EngineBroke();
        }
        if(currentRPM < 0)
            {
                EngineStall();
            }
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

        
    }

    void UpdateWheel(WheelCollider col, Transform trans)
    {
        Vector3 position;
        Quaternion rotation;
        col.GetWorldPose(out position, out rotation);

        trans.position = position;
        trans.rotation = rotation;
    }
    void EngineBroke()
    {
        currentRPM = 0f;
        engineIsBroke = true;
    }
    void EngineStall()
    {
        currentRPM = 0f;
        engineOn = false;
    }
}
