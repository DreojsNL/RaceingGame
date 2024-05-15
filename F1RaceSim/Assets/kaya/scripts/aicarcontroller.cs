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
        
        currentaccel = accel * Input.GetAxis("Vertical");

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
        var angle = (Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg);
        //transform.rotation = Quaternion.AngleAxis(angle, -Vector3.up);
        var lookrot = Quaternion.LookRotation(dir);
        
        Debug.Log(angle);

        
        if (lookrot.y > transform.rotation.y)
        {
            currentturnpower = maxturnpower * -1;
        }
        if (lookrot.y < transform.rotation.y)
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
                current = 0;
                laps += 1;
                //checkpoints.Remove(checkpoints[0]);
                //Destroy(other.gameObject);
            }
            else
            {
                current += 1;
            }
                
        }
    }
    
}
