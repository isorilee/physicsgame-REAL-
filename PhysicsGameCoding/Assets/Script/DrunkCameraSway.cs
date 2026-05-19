using UnityEngine;

public class DrunkCameraSway : MonoBehaviour
{
    public enum DrunkState
    {
        Sober,
        Tipsy,
        Drunk,
        Wasted
    }

    [Header("Drunk State")]
    public DrunkState currentState = DrunkState.Sober;

    [Header("Sway Amount")]
    public float soberAmount = 0f;
    public float tipsyAmount = 0.03f;
    public float drunkAmount = 0.08f;
    public float wastedAmount = 0.15f;

    [Header("Sway Speed")]
    public float soberSpeed = 0f;
    public float tipsySpeed = 1.5f;
    public float drunkSpeed = 2.5f;
    public float wastedSpeed = 4f;

    private Vector3 startLocalPosition;
    private float currentAmount;
    private float currentSpeed;

    void Start()
    {
        startLocalPosition = transform.localPosition;
        ApplyDrunkState(currentState);
    }

    void LateUpdate()
    {
        float swayX = Mathf.Sin(Time.time * currentSpeed) * currentAmount;

        transform.localPosition = startLocalPosition + new Vector3(swayX, 0f, 0f);
    }

    public void ApplyDrunkState(DrunkState state)
    {
        currentState = state;

        switch (state)
        {
            case DrunkState.Sober:
                currentAmount = soberAmount;
                currentSpeed = soberSpeed;
                break;

            case DrunkState.Tipsy:
                currentAmount = tipsyAmount;
                currentSpeed = tipsySpeed;
                break;

            case DrunkState.Drunk:
                currentAmount = drunkAmount;
                currentSpeed = drunkSpeed;
                break;

            case DrunkState.Wasted:
                currentAmount = wastedAmount;
                currentSpeed = wastedSpeed;
                break;
        }
    }
}