using UnityEngine;

public class DrunkShaderController : MonoBehaviour
{
    [Header("Shader Material")]
    public Material drunkMaterial;

    [Header("Shader Property Names")]
    public string distortionStrengthProperty = "_distortionStrength";
    public string distortionSpeedProperty = "_distortionSpeed";

    [Header("Effect Range")]
    public float maxDistortionStrength = 0.08f;
    public float minDistortionSpeed = 0.5f;
    public float maxDistortionSpeed = 4f;

    void Update()
    {
        if (drunkMaterial == null) return;

        float drunkAmount = 0f;

        // Gets drunkness from DrunkManager and keeps it between 0 and 1
        if (DrunkManager.instance != null && DrunkManager.instance.isDrunk)
        {
            drunkAmount = Mathf.Clamp01(DrunkManager.instance.drunkLevel);
        }

        // Stronger distortion as drunkness increases
        float distortionStrength = drunkAmount * maxDistortionStrength;

        // Faster distortion as drunkness increases
        float distortionSpeed = Mathf.Lerp(minDistortionSpeed, maxDistortionSpeed, drunkAmount);

        drunkMaterial.SetFloat(distortionStrengthProperty, distortionStrength);
        drunkMaterial.SetFloat(distortionSpeedProperty, distortionSpeed);
    }
}
