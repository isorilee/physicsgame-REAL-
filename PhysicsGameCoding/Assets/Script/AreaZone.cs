using UnityEngine;
using Unity.Cinemachine;

public class AreaZone : MonoBehaviour
{
    [Header("Camera Confiner")]
    public CinemachineConfiner3D confiner;

    [Header("Camera Bounds")]
    public Collider cameraBounds;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entered area zone: " + other.name);

        if (!other.CompareTag("Player"))
            return;

        SwitchCameraBounds();
    }

    void SwitchCameraBounds()
    {
        if (confiner == null)
        {
            Debug.LogWarning("AreaZone: confiner not assigned.");
            return;
        }

        if (cameraBounds == null)
        {
            Debug.LogWarning("AreaZone: cameraBounds not assigned.");
            return;
        }

        // Switch the camera bounds for this area.
        confiner.BoundingVolume = cameraBounds;

        Debug.Log("AreaZone: switched bounds to " + cameraBounds.name);
    }
}