using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CarController : MonoBehaviour
{
    // Wheel colliders
    public WheelCollider backLeft;
    public WheelCollider backRight;
    public WheelCollider frontLeft;
    public WheelCollider frontRight;

    // Wheel transforms
    public Transform backLeftM;
    public Transform backRightM;
    public Transform frontLeftM;
    public Transform frontRightM;

    // UI Texts
    public TextMeshProUGUI engineStatus;
    public TextMeshProUGUI gearText;
    public TextMeshProUGUI kmhText;
    public TextMeshProUGUI rmpText;
    public TextMeshProUGUI tcText;

    // Car parameters
    public AnimationCurve hpToRPMCurve;
    public float acceleration = 500f;
    public float brakeForce = 300f;
    public float differentialRatio;
    public float idleRPM = 1000f;
    public float maxRPM = 6000f;
    public float maxTurnAngle = 15;

    // Engine related variables
    private int shiftRandom;
    private bool shiftInputPressed = false;
    private int maxGear = 5;
    public bool engineIsBroke;
    public bool engineOn;
    public bool tractionControlOn = true; // Traction Control toggle
    public float gasInput;
    public float brakeInput;
    public float ignitonInput;
    public float shiftInput;
    private float clutch;
    private float clutchInvers;
    private float currentAcceleration = 0f;
    private float currentBrakeForce = 0f;
    private float currentRPM = 0f;
    private float currentTurnAngle = 0f;
    public int currentGear = 1;
    public float[] gearRatios = { 3.5f, 2.5f, 1.8f, 1.4f };

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        ignitonInput = Input.GetAxis("Ignition");
        brakeInput = Input.GetAxis("Brake");
        shiftRandom = Random.Range(1000, 2500);
        shiftInput = Input.GetAxis("Shitft");
        if (tractionControlOn)
        {
            tcText.text = "TC:on";
        }
        else
        {
            tcText.text = "TC:off";
        }
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
        if (ignitonInput == 1 && clutch == 2) // startCar
        {
            engineOn = !engineOn;
        }
        if(ignitonInput == 0 && clutch == 2 && currentRPM == 0)
        {
            engineOn = false;
        }

        if (!engineOn)
        {
            currentRPM = Mathf.Lerp(currentRPM, 0f, 30 * Time.deltaTime);
        }

        gearText.text = currentGear.ToString();

        clutchInvers = Input.GetAxis("Clutch");
        clutch = 1f - clutchInvers;

        kmhText.text = "KMH:" + (rb.velocity.magnitude * 3.6f).ToString(format: "000");
        if (Input.GetAxisRaw("Shitft") > 0)
        {
            if (!shiftInputPressed)
            {
                shiftInputPressed = true;
                ShiftUp();
            }
        }
        else if(Input.GetAxisRaw("Shitft") < 0)
        {
            shiftInputPressed = true;
            ShiftDown();
        }
        else
        {
            shiftInputPressed = false;
        }

        // Toggle Traction Control
        if (Input.GetKeyDown(KeyCode.T))
        {
            tractionControlOn = !tractionControlOn;
        }
    }
    void ShiftUp()
    {
        if (currentGear <= maxGear && clutch == 2) // Assuming maxGear is the maximum gear
        {
            currentGear++;
            if (!engineIsBroke)
            {
                currentRPM -= shiftRandom;
            }
        }
    }

    void ShiftDown()
    {
        if (currentGear > 1 && clutch == 2)
        {
            currentGear--;
            if (!engineIsBroke)
            {
                currentRPM += shiftRandom;
            }
        }
    }
    private void FixedUpdate()
    {
        string currentRPMstring = currentRPM.ToString(format: "0000");
        rmpText.text = "RPM:" + currentRPMstring;

        // Control steering
        currentTurnAngle = maxTurnAngle * Input.GetAxis("Horizontal");
        frontRight.steerAngle = currentTurnAngle;
        frontLeft.steerAngle = currentTurnAngle;

        if (!engineIsBroke && engineOn)
        {
            gasInput = Input.GetAxis("Vertical");

            if (clutch == 1)
            {
                int random = Random.Range(-50, 50);
                float wheelRPM = Mathf.Abs((backRight.rpm + backLeft.rpm) / 2f) * gearRatios[currentGear] * differentialRatio;
                currentRPM = Mathf.Lerp(currentRPM, Mathf.Max(idleRPM - 100, wheelRPM), Time.deltaTime * 3f);
                currentAcceleration = (hpToRPMCurve.Evaluate(currentRPM / maxRPM) * acceleration / currentRPM) * gearRatios[currentGear] * differentialRatio * 5252f * clutch;
            }
            else
            {
                int random = Random.Range(-50, 50);
                currentRPM = Mathf.Lerp(currentRPM, Mathf.Max(idleRPM, maxRPM * gasInput) + random, Time.deltaTime * .5f);
                currentAcceleration = 0f;
            }
            if (currentRPM < maxRPM && currentGear != 0 && engineOn)
            {
                // Apply traction control if enabled
                if (tractionControlOn)
                {
                    // Adjust gasInput based on current RPM
                    gasInput *= Mathf.Clamp01((maxRPM - currentRPM) / maxRPM);
                }

                backLeft.motorTorque = currentAcceleration * gasInput;
                backRight.motorTorque = currentAcceleration * gasInput;
            }
            else
            {
                backLeft.motorTorque = 0f;
                backRight.motorTorque = 0f;
            }

            if (gasInput == -1 || brakeInput > 0)
            {
                currentBrakeForce = brakeForce;
            }

            if (currentRPM < 1000 && clutch == 1)
            {
                EngineStall();
            }

            if (currentRPM > maxRPM && gasInput == 0)
            {
                EngineBroke();
            }

            if (currentRPM < 0)
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

    private void UpdateWheel(WheelCollider col, Transform trans)
    {
        Vector3 position;
        Quaternion rotation;
        col.GetWorldPose(out position, out rotation);

        trans.position = position;
        trans.rotation = rotation;
    }

    private void EngineBroke()
    {
        currentBrakeForce = 3000;
        engineOn = false;
        engineIsBroke = true;
    }

    private void EngineStall()
    {
        engineOn = false;
    }
}
