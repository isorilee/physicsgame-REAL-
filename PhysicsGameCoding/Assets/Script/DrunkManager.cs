using UnityEngine;

public class DrunkManager : MonoBehaviour
{
    
    public static DrunkManager instance;
    
    [Header("Drunk State")]
    public bool isDrunk = false;

    //0=sober,1=verydrunk
    [Range(0f, 1f)]
    public float drunkLevel = 0f;

     void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        else
        {
            //prevent duplicate DrunkManagers in the scene 
            Destroy(gameObject);
        }
    }


    public void SetDrunk(float level)
    {
        isDrunk = true;
        drunkLevel = Mathf.Clamp01(level);
    }

    public void SetSober()
    {
        isDrunk = false;
        drunkLevel = 0f;

    }
}
