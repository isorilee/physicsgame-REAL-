using UnityEngine;

public class DrunkManager : MonoBehaviour
{
    public static DrunkManager instance;

    public bool isDrunk = false;
    public float drunkLevel = 0f;

    [Header("Drunk Progression")]
    public float maxDrunkLevel = 5f;
    public float increaseRate = 0.2f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (isDrunk)
        {
            drunkLevel += increaseRate * Time.deltaTime;
            drunkLevel = Mathf.Clamp(drunkLevel, 1f, maxDrunkLevel);
        }
    }

    public void StartDrunkMode()
    {
        isDrunk = true;
        drunkLevel = 1f;

        Debug.Log("Drunk mode started.");
    }
}