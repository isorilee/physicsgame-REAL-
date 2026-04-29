using UnityEngine;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

public class GameFlowManager : MonoBehaviour
{
    public enum GameState
    {
        BarIntro,
        PaymentMission,
        MirrorMission,
        BathroomMission

    }

    [Header("Current Game State")]
    public GameState currentState;

    [Header("Cinemachine Cameras")]
    public CinemachineCamera firstPersonCamera;
    public CinemachineCamera thirdPersonCamera;

    [Header("Camera Priority")]
    public int activePriority = 20;
    public int inactivePriority = 0;

    [Header("Managers")]
    public DrunkManager drunkManager;
    public MissionUI missionUI;

    void Start()
    {
        SetState(GameState.BarIntro);
    }

    void Update()
    {
        //Temp test: press E to move to the next level
        //if (Input.GetKeyUp(KeyCode.E))
        //{
        //    GoToNextState();
        //}
        if(Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {

            GoToNextState();
        }


    }

    public void GoToNextState()
    {
        if (currentState == GameState.BarIntro)
        {
            SetState(GameState.PaymentMission);
        }

        else if (currentState == GameState.PaymentMission)
        {
            SetState(GameState.MirrorMission);
        }

        else if (currentState == GameState.MirrorMission)
        {
            SetState(GameState.BathroomMission);
        }

        else if (currentState == GameState.BathroomMission)
        {
            Debug.Log("Game Over");

        }
    
    }


    public void SetState(GameState newState)
    {
        currentState = newState; 

        switch (currentState)
        {
            case GameState.BarIntro:
                SwitchToFirstPerson();

                drunkManager.SetSober();
                missionUI.SetMissionText("pour yourself a drink.");
                Debug.Log("state: Bar Intro");
                break;

                case GameState.PaymentMission:
                SwitchToThirdPerson();

                drunkManager.SetDrunk(0.35f);
                missionUI.SetMissionText("pay the bill");
                Debug.Log("Friend NPC: should we get the check?");
                break;

                case GameState.MirrorMission:
                SwitchToThirdPerson();
                drunkManager.SetDrunk(0.6f);
                missionUI.SetMissionText("make yourself presentable");
                Debug.Log("you look... horrible");
                break;

                case GameState.BathroomMission:
                SwitchToThirdPerson();
                drunkManager.SetDrunk(0.9f);
                missionUI.SetMissionText("Find the bathroom");
                Debug.Log("You don't have an extra pair of pants...");
                break;
        }

    }

    void SwitchToFirstPerson()
    {
        firstPersonCamera.Priority.Value = activePriority;
        thirdPersonCamera.Priority.Value = inactivePriority;

    }

    void SwitchToThirdPerson()
    {
        firstPersonCamera.Priority.Value = inactivePriority;
        thirdPersonCamera.Priority.Value = activePriority;

    }


}
