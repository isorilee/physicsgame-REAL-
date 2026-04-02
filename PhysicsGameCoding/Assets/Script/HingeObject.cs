using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
public class HingeObject : MonoBehaviour
{

    public float minAngle = 0f;
    //the min angle the hinge can rotate to 0=closed 
    public float maxAngle = 90f;
    //the max angle the hinge can rotate to 90 = fully opened 
    public bool useSpring = false;
    //if true the hinge will psring back toward the rest angle when released 
    public float springTargetAngle; //the angle the spring tries to return to 
    public float springForce; //how strong the spring force is 
    public float springDamper; //how much the spring dampens 

    //events 
    public UnityEvent OnReachMax; //fired when the hinge reaches or passes the max angle 
    public UnityEvent OnReachMin;//fired when the hinge returns to or passes the min angle 

    //something more to be added

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        hinge = GetComponent<HingeJoint>();
        ConfigureHinge();
    }

    // Update is called once per frame
    void Update()
    {
        //check if we hit the limits should fire puzzle events 
        float currentAngle = hinge.angle;

        if (!maxEventFired && currentAngle >= maxAngle - eventThreshold)
        {
            maxEventFired = true;
            minEventFired = false; 


        }

    }

   //configure hinge 
   //sets up joint limits and spring through code
    void ConfigureHinge()
    {
        //limits 
        //jointlimits is a struct we have to set all fields then assign it back 

        JointLimits limits = hinge.limits; 
        limits.min = minAngle;
        limits.max = maxAngle;
        limits.bounciness = 0f; 
        limits.bounceMinVelocity = 0.2f;
        //hinge.limits = limits; 
        //hinge.useLimits = true
        //



        if (useSpring)
        {
            
        } 
    
    
    }


    public void DriveToMax()
    {
        SetMotorTarget(maxAngle);

    }

    public void DriveToMin()
    {



    }





}
