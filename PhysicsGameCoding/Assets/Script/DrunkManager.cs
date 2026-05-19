using UnityEngine;

public class DrunkManager : MonoBehaviour
{
    public static DrunkManager instance;

    public enum DrunkState
    {
        Sober,
        Tipsy,
        Drunk,
        Wasted
    }

    [Header("Drunk Level")]
    [Range(0f, 1f)]
    public float drunkLevel = 0f;

    [Header("Drunk State")]
    public DrunkState currentState = DrunkState.Sober;

    public bool isDrunk = false;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        SetDrunk(drunkLevel);
    }

    public void Drink()
    {
        SetDrunk(drunkLevel + 0.35f);
    }

    public void SetDrunk(float newLevel)
    {
        drunkLevel = Mathf.Clamp01(newLevel);
        isDrunk = drunkLevel > 0f;
        UpdateDrunkState();
    }

    public void SetDrunkState(DrunkState newState)
    {
        currentState = newState;

        switch (newState)
        {
            case DrunkState.Sober:
                drunkLevel = 0f;
                break;
            case DrunkState.Tipsy:
                drunkLevel = 0.25f;
                break;
            case DrunkState.Drunk:
                drunkLevel = 0.6f;
                break;
            case DrunkState.Wasted:
                drunkLevel = 1f;
                break;
        }

        isDrunk = drunkLevel > 0f;
    }

    public void SetState(DrunkState newState)
    {
        SetDrunkState(newState);
    }

    private void UpdateDrunkState()
    {
        if (drunkLevel <= 0f)
            currentState = DrunkState.Sober;
        else if (drunkLevel < 0.35f)
            currentState = DrunkState.Tipsy;
        else if (drunkLevel < 0.75f)
            currentState = DrunkState.Drunk;
        else
            currentState = DrunkState.Wasted;
    }
}