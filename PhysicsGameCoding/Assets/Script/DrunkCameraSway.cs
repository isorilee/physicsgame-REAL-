using UnityEngine;

public class DrunkCameraSway : MonoBehaviour
{
    public float swayAmount = 2f;
    public float swaySpeed = 1.5f;
    public float bobAmount = 0.05f;

    private Vector3 startLocalPos;

    void Start()
    {
        startLocalPos = transform.localPosition;
    }

    void Update()
    {
        //if (DrunkManager.instance == null || !DrunkManager.instance.isDrunk)
        {
            transform.localPosition = startLocalPos;
            return;
        }

        //float drunkLevel = DrunkManager.instance.drunkLevel;

        //float swayX = Mathf.Sin(Time.time * swaySpeed) * swayAmount * drunkLevel;
        //float swayY = Mathf.Cos(Time.time * swaySpeed * 1.3f) * swayAmount * 0.5f * drunkLevel;
        //float bobY = Mathf.Sin(Time.time * swaySpeed * 2f) * bobAmount * drunkLevel;

        //transform.localRotation = Quaternion.Euler(swayY, 0f, swayX);
        //transform.localPosition = startLocalPos + new Vector3(0f, bobY, 0f);
    }
}