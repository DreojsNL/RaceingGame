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
    public GameObject steeringWheel;
    public AnimationCurve hpToRPMCurve;
    public float acceleration = 500f;
    public float brakeForce = 300f;
    public float differentialRatio;
    public float idleRPM = 1000f;
    public float maxRPM = 6000f;
    public float maxTurnAngle = 15;
    public BreakingPart spolierPart;
    public BreakingPart lipPart;
    private float kmhNumber;

    // Engine related variables
    private int shiftRandom;
    private bool shiftInputPressed = false;
    private int maxGear = 5;
    public bool engineIsBroke;
    public bool engineOn;
    public bool tractionControlOn = true; // Traction Control toggle
    private float gasInput;
    public float brakeInput;
    private float ignitonInput;
    public float shiftInput;
    private float clutch;
    private float clutchInvers;
    private float currentAcceleration = 0f;
    private float currentRPM = 0f;
    private float currentTurnAngle = 0f;
    private bool ignitionProcessed = false;
    public int currentGear = 1;
    public float steeringSensitivity = 1.0f;
    public float[] gearRatios = { 3.5f, 2.5f, 1.8f, 1.4f };
    private bool isReversed;
    public float downForce;
    public AudioSource engineAudio;
    public float minPitch = 0.5f;
    public float maxPitch = 2.0f;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        engineAudio = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (engineIsBroke)
        {
            engineOn = false;
        }
        kmhNumber = rb.velocity.magnitude * 3.6f;
        UpdateEngineAudioPitch();
        UpdateSteeringWheel();
        ignitonInput = Input.GetAxis("Ignition");
        brakeInput = Input.GetAxis("Brake");
        shiftRandom = Random.Range(1000, 2500);
        shiftInput = Input.GetAxis("Shitft");
        if (!engineOn)
        {
            engineAudio.enabled = false;
        }
        else
        {
            engineAudio.enabled = true;
        }
        if(!lipPart.broken && !spolierPart.broken)
        {
            downForce = 4f;
            rb.drag = 0.05f;
        }
        else if(!lipPart.broken || !spolierPart.broken)
        {
            downForce = 2f;
            rb.drag = 0.1f;
        }
        else
        {
            rb.drag = 0.12f;
            downForce = 0f;
        }
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
        if (ignitonInput == 1 && clutch == 2 && !ignitionProcessed)
        {
            engineOn = !engineOn;
            ignitionProcessed = true; // Mark the ignition input as processed
        }
        else if (!(ignitonInput == 1 && clutch == 2))
        {
            ignitionProcessed = false; // Reset the flag if the condition is not met
        }
        if (ignitonInput == 0 && clutch == 2 && currentRPM == 0)
        {
            engineOn = false;
        }

        if (!engineOn)
        {
            currentRPM = Mathf.Lerp(currentRPM, 0f, 30 * Time.deltaTime);
        }
        if (!isReversed)
        {
            gearText.text = currentGear.ToString();
        }
        

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
            if (!shiftInputPressed)
            {
                shiftInputPressed = true;
                ShiftDown();
            }
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
        if (isReversed && clutch == 2)
        {
            acceleration = acceleration * -1;
            isReversed = false;
            currentGear--;
            if (!engineIsBroke)
            {
                currentRPM -= shiftRandom;
            }
        }
        if (currentGear <= maxGear && clutch == 2 && !isReversed) // Assuming maxGear is the maximum gear
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
        if (isReversed && clutch == 2)
        {
            Debug.Log("No");
        }
        if (currentGear > 1 && clutch == 2 && !isReversed)
        {
            currentGear--;
            if (!engineIsBroke)
            {
                currentRPM += shiftRandom;
            }
        }
        else if(clutch == 2)
        {
            isReversed = true;
            gearText.text = "R";
            acceleration = acceleration * -1;
        }
    }
    private void UpdateEngineAudioPitch()
    {
        // Normalize current RPM within the range of idleRPM and maxRPM
        float normalizedRPM = Mathf.InverseLerp(idleRPM, maxRPM, currentRPM);

        // Map the normalized RPM to the pitch range
        float targetPitch = Mathf.Lerp(minPitch, maxPitch, normalizedRPM);

        // Set the audio source pitch
        engineAudio.pitch = targetPitch;
    }
    private void FixedUpdate()
    {
        ApplyDownforce();
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
                // Engine braking when the brakes are applied
                if (brakeInput > 0)
                {
                    currentRPM -= brakeForce * Time.deltaTime;
                }
                else
                {
                    // Engine acceleration
                    int random = Random.Range(-50, 50);
                    float wheelRPM = Mathf.Abs((backRight.rpm + backLeft.rpm) / 2f) * gearRatios[currentGear] * differentialRatio;
                    currentRPM = Mathf.Lerp(currentRPM, Mathf.Max(idleRPM - 100, wheelRPM), Time.deltaTime * 3f);
                    currentAcceleration = (hpToRPMCurve.Evaluate(currentRPM / maxRPM) * acceleration / currentRPM) * gearRatios[currentGear] * differentialRatio * 5252f * clutch;
                }
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
        if (brakeInput == 1)
        {
            gasInput = 0f;
            ApplyBrakes();
        }
        else
        {
            frontRight.brakeTorque = 0;
            frontLeft.brakeTorque = 0;
            backLeft.brakeTorque = 0;
            backRight.brakeTorque = 0;
        }

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
        brakeForce = 3000;
        ApplyBrakes();
        engineOn = false;
        engineIsBroke = true;
    }
    private void ApplyBrakes()
    {
        // Apply brakes to all wheels
        frontRight.brakeTorque = brakeForce;
        frontLeft.brakeTorque = brakeForce;
        backLeft.brakeTorque = brakeForce;
        backRight.brakeTorque = brakeForce;
    }
    private void EngineStall()
    {
        engineOn = false;
    }
    private void UpdateSteeringWheel()
    {
        // Calculate the rotation angle based on the current turn angle and sensitivity
        float rotationAngle = currentTurnAngle * steeringSensitivity;

        // Apply rotation to the steering wheel around its up axis
        steeringWheel.transform.localRotation = Quaternion.Euler(
            steeringWheel.transform.localRotation.eulerAngles.x,
            rotationAngle,
            steeringWheel.transform.localRotation.eulerAngles.z
        );
    }
    private void ApplyDownforce()
    {
        // Calculate downforce based on car parameters and current speed
        float currentDownforce = rb.velocity.magnitude * rb.velocity.magnitude * downForce;

        // Apply downforce in the opposite direction of the car's velocity (to simulate aerodynamic drag)
        rb.AddForce(-transform.up * currentDownforce);
    }
    private void OnCollisionEnter(Collision collision)
    {
        // Get the total force applied by the collision
        float collisionForce = collision.impulse.magnitude / Time.fixedDeltaTime;

        // Determine which part should break based on collision force
        if (collisionForce >= 1000) // Adjust the threshold as needed
        {
            Debug.Log("Collision force: " + collisionForce);
            Debug.Log("Crash - Lip broken");
            lipPart.broken = true;
            // Decide which part should break based on collision force
            if (collisionForce >= 2000)
            {
                Debug.Log("Crash - Spoiler broken");
                spolierPart.broken = true;
                Debug.Log("Crash - Lip broken");
                lipPart.broken = true;
            }
        }
    }

}
