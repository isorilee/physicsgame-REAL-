using UnityEngine;
using System.Collections;

public class InteractableObject : MonoBehaviour
{
    [Header("Highlight")]
    public Color highlightColor = Color.yellow;
    [Range(0f, 1f)] public float highlightStrength = 0.5f;

    [Header("Interaction Type")]
    public bool isDrink = false;

    [Header("Drink Settings")]
    public float drinkDelay = 0.75f;
    public GameObject objectToHideAfterUse;

    private Renderer objectRenderer;
    private Color originalColor;
    private bool isHighlighted = false;
    private bool hasBeenUsed = false;

    void Awake()
    {
        objectRenderer = GetComponentInChildren<Renderer>();

        if (objectRenderer != null)
        {
            originalColor = objectRenderer.material.color;
        }
        else
        {
            Debug.LogWarning(gameObject.name + " has no renderer for highlight.");
        }
    }

    public void Highlight()
    {
        if (isHighlighted || objectRenderer == null) return;

        objectRenderer.material.color = Color.Lerp(originalColor, highlightColor, highlightStrength);
        isHighlighted = true;
    }

    public void Unhighlight()
    {
        if (!isHighlighted || objectRenderer == null) return;

        objectRenderer.material.color = originalColor;
        isHighlighted = false;
    }

    public void Interact()
    {
        Debug.Log("Interact called on: " + gameObject.name);

        if (hasBeenUsed) return;

        if (isDrink)
        {
            //StartCoroutine(DrinkSequence());
        }
        else
        {
            Debug.Log("Interacted with " + gameObject.name + ", but it is not marked as a drink.");
        }
    }

    private IEnumerator DrinkSequence()
    {
        hasBeenUsed = true;

        Debug.Log("Drinking...");

        yield return new WaitForSeconds(drinkDelay);

        if (DrunkManager.instance != null)
        {
            //DrunkManager.instance.StartDrunkMode();
            Debug.Log("Player is now drunk.");
        }
        else
        {
            Debug.LogError("No DrunkManager found in scene.");
        }

        Unhighlight();

        if (objectToHideAfterUse != null)
        {
            objectToHideAfterUse.SetActive(false);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}

