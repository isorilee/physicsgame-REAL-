using UnityEngine;
using Unity.Cinemachine;

public class CameraZoneTrigger : MonoBehaviour
{
    [Header("Confiner")]
    public CinemachineConfiner3D Confiner;

    [Header("Target Bounds")]
    public Collider targetBounds;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Confiner.BoundingVolume = targetBounds;
        //refresh the confiner after changing bounds 
        Confiner.enabled = false;
        Confiner.enabled = true;
        Debug.Log("Switched to:" + targetBounds.name);
    }
}
