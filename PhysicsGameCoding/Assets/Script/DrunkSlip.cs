using UnityEngine;

public class DrunkSlip : MonoBehaviour
{
    public Rigidbody rb;
    public float slipForce = 2f;
    public float slipSpeed = 1.8f;

    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
    }

    //void FixedUpdate()
    //{
    //    if (DrunkManager.instance == null || !DrunkManager.instance.isDrunk || rb == null) return;

    //    float level = DrunkManager.instance.drunkLevel;
    //    float sideSlip = Mathf.Sin(Time.time * slipSpeed) * slipForce * level;

    //    rb.AddForce(transform.right * sideSlip, ForceMode.Force);
    //}
}