using System.Collections;
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
    
    [Header("Drunk State")]
    public DrunkState currentState = DrunkState.Sober;

    //0=sober,1=verydrunk
    [Range(0f, 1f)]
    public float drunkLevel = 0f;

    public bool isDrunk => currentState != DrunkState.Sober;

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

    public void SetDrunkState(DrunkState state)
    {
        currentState = state;

        switch(state)
        {
            case DrunkState.Sober:
                drunkLevel = 0f;
                break;

            case DrunkState.Tipsy:
                drunkLevel = 0.3f;
                break;

            case DrunkState.Drunk:
                drunkLevel = 0.6f;
                break;

            case DrunkState.Wasted:
                drunkLevel = 0.9f;
                break; 

        }

        Debug.Log("Drunk State: " + currentState);
    }
        
       


}
    //public void SetDrunk(float level)
    //{
    //    isDrunk = true;
    //    drunkLevel = Mathf.Clamp01(level);
    //}

    //public void SetSober()
    //{
    //    isDrunk = false;
    //    drunkLevel = 0f;

    //}

