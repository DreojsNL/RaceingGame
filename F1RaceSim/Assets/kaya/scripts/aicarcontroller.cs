using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;

public class aicarcontroller : MonoBehaviour
{
    public float deadzone;
    public int laps;

    public float timelap;
    public float time;

    public float fastesttimelap;
    
    public float brakedeadzone;

    public int current;
    public List<Transform> checkpoints = new List<Transform>();
   
    float f = 0;
    public float turnspeed;

    public WheelCollider frontRight;
    public WheelCollider frontLeft;
    public WheelCollider backRight;
    public WheelCollider backLeft;

    public Transform frontRightM;
    public Transform frontLeftM;
    public Transform backRightM;
    public Transform backLeftM;

    public float accel;
    public float currentaccel;
    public float breakpower;
    public float currentbreakpower;

    public float maxturnpower;
    public float currentturnpower;

    public bool breaking;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TimerRoutine());
    }
    IEnumerator TimerRoutine()
    {
        WaitForSeconds delay = new WaitForSeconds(1);
        while (true)
        {
            timelap += 1;
            yield return delay;
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        
        currentaccel = accel * 1;

        if (breaking)
        {
            currentbreakpower = breakpower;
        }
        else
        {
            currentbreakpower = 0;
        }

        frontRight.motorTorque = currentaccel;
        frontLeft.motorTorque = currentaccel;

        backRight.brakeTorque = currentbreakpower;
        backLeft.brakeTorque = currentbreakpower;
        frontLeft.brakeTorque = currentbreakpower;
        frontRight.brakeTorque = currentbreakpower;


        Vector3 localtoaicheckpoint2 = gameObject.transform.InverseTransformPoint(checkpoints[current + 1].transform.position);

        if (localtoaicheckpoint2.x > brakedeadzone)
        {
            accel = -300;
        }
        else if (localtoaicheckpoint2.x < -brakedeadzone)
        {
            accel = -300;

        }
        else
        {
            accel = -600;
        }

        var dir = checkpoints[current].position - gameObject.transform.position;
        
        
        var lookrot = Quaternion.LookRotation(dir);
        Vector3 localtoaicheckpoint = gameObject.transform.InverseTransformPoint(checkpoints[current].transform.position);


        if (localtoaicheckpoint.x > deadzone)
        {
            currentturnpower = maxturnpower * -1;
        }
        else if (localtoaicheckpoint.x < -deadzone)
        {
            currentturnpower = maxturnpower * 1;
        }
        else
        {
            currentturnpower = 0;
        }




        UpdateWheel(frontLeft, frontLeftM);
        UpdateWheel(frontRight, frontRightM);
        UpdateWheel(backLeft, backLeftM);
        UpdateWheel(backRight, backRightM);

        frontLeft.steerAngle = currentturnpower;
        frontRight.steerAngle = currentturnpower;
    }

    







        


    private void UpdateWheel(WheelCollider col, Transform trans)
    {
        Vector3 position;
        Quaternion rotation;
        col.GetWorldPose(out position, out rotation);

        trans.position = position;
        trans.rotation = rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "checkpoint")
        {
            if (current >= checkpoints.ToArray().Length -1)
            {
                foreach(var check in checkpoints)
                {
                    check.gameObject.SetActive(true);
                }
                current = 0;
                laps += 1;
                time = timelap;
                timelap = 0;
                if (time < fastesttimelap)
                {
                    fastesttimelap = time;
                }
            }
            else
            {
                checkpoints[current].gameObject.SetActive(false);

                current += 1;
            }
                
        }
    }
    
}
