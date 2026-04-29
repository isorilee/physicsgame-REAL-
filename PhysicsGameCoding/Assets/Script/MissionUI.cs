using UnityEngine;
using TMPro;

public class MissionUI : MonoBehaviour
{
    [Header("UI Text")]

    public TMP_Text missionText;

    public void SetMissionText(string text)
    {
        missionText.text = text; 

    }
}
