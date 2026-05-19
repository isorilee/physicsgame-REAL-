using UnityEngine;

public class DrunkRagdollJointController : MonoBehaviour
{
    [Header("Upper Body Joints Only")]
    public ConfigurableJoint[] upperBodyJoints;

    public float soberSpring = 2500f;
    public float tipsySpring = 1800f;
    public float drunkSpring = 1200f;
    public float wastedSpring = 700f;

    public float damper = 150f;
    public float maxForce = 99999f;

    private DrunkManager drunkManager;

    private void Start()
    {
        drunkManager = FindFirstObjectByType<DrunkManager>();
    }

    private void FixedUpdate()
    {
        if (drunkManager == null) return;

        float spring = GetSpringByState(drunkManager.currentState);

        foreach (ConfigurableJoint joint in upperBodyJoints)
        {
            if (joint == null) continue;

            JointDrive drive = new JointDrive
            {
                positionSpring = spring,
                positionDamper = damper,
                maximumForce = maxForce
            };

            joint.angularXDrive = drive;
            joint.angularYZDrive = drive;
        }
    }

    private float GetSpringByState(DrunkManager.DrunkState state)
    {
        switch (state)
        {
            case DrunkManager.DrunkState.Sober:
                return soberSpring;

            case DrunkManager.DrunkState.Tipsy:
                return tipsySpring;

            case DrunkManager.DrunkState.Drunk:
                return drunkSpring;

            case DrunkManager.DrunkState.Wasted:
                return wastedSpring;

            default:
                return soberSpring;
        }
    }
}