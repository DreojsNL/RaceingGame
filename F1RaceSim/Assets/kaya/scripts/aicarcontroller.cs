using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aicarcontroller : MonoBehaviour
{

    public int laps;


    public float fuel;
    public float normalspeed;
    public float speed;
    public float lowfuelspeed;

    public Transform rotation;
    public Transform pos;
    public Vector3 move;
    public int current;
    public List<Transform> checkpoints = new List<Transform>();
    public CharacterController characterController;
    float f = 0;
    public float turnspeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        fuel--;
        if (fuel > 0)
        {
            if (fuel < 100)
            {
                speed = lowfuelspeed;
            }
            if (fuel <= 0)
            {
                speed = 0;
            }
            if (fuel > 100)
            {
                speed = normalspeed;
            }

            var dir = checkpoints[current].position - pos.position;
            var angle = (Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg);
            var anglei = transform.rotation.y;
            if (anglei < angle)
            {
                
                f = turnspeed;
                
                
                transform.rotation = Quaternion.AngleAxis(angle + f, -Vector3.up);
            }
            else if (anglei > angle)
            {
                
                f = turnspeed;


                transform.rotation = Quaternion.AngleAxis(angle - f, -Vector3.up);
            }
            
            move = transform.right;
            characterController.Move(move * Time.deltaTime * speed);
        }
        
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
