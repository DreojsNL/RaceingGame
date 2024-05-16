using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class aicarcontroller : MonoBehaviour
{

    public int laps;


    public float fuel;
    public float normalspeed;
    public float speed;
    public float lowfuelspeed;
    public int current;
    public List<Transform> checkpoints = new List<Transform>();
    public CharacterController characterController;
    float f = 0;
    public float turnspeed;

    public WheelCollider frontright;
    public WheelCollider frontleft;
    public WheelCollider backright;
    public WheelCollider backleft;

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

        frontright.motorTorque = currentaccel;
        frontleft.motorTorque = currentaccel;

        backright.brakeTorque = currentbreakpower;
        backleft.brakeTorque = currentbreakpower;
        frontleft.brakeTorque = currentbreakpower;
        frontright.brakeTorque = currentbreakpower;

        
        


        var dir = checkpoints[current].position - gameObject.transform.position;
        
        
        var lookrot = Quaternion.LookRotation(dir);
        Vector3 localtoaicheckpoint = gameObject.transform.InverseTransformPoint(checkpoints[current].transform.position);

        Debug.Log(lookrot.y + transform.rotation.y);

        if (localtoaicheckpoint.x > 0)
        {
            currentturnpower = maxturnpower * -1;
        }
        else if (localtoaicheckpoint.x < 0)
        {
            currentturnpower = maxturnpower * 1;
        }










        frontleft.steerAngle = currentturnpower;
        frontright.steerAngle = currentturnpower;


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
                
            }
            else
            {
                checkpoints[current].gameObject.SetActive(false);

                current += 1;
            }
                
        }
    }
    
}
