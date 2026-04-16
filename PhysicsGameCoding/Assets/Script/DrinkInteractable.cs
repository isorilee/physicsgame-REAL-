using UnityEngine;
using System.Collections;

public class DrinkInteractable : MonoBehaviour
{
    // the color of object glows when the player looks at it
    public Color highlightColor = new Color(1f, 0.95f, 0.6f);
    [Range(0, 1f)] public float highlightStrength = 0.4f;

    [Header("Interaction")]
    public bool isDrink = false;
    public bool canOnlyUseOnce = true;
    private bool hasBeenUsed = false;

    [Header("Drink Settings")]
    public GameObject drinkVisual;
    public float drinkDelay = 1f;

    private Renderer objectRenderer;
    private Color originalColor;
    private bool isHighlighted = false;

    void Awake()
    {
        objectRenderer = GetComponent<Renderer>();

        if (objectRenderer != null)
        {
            originalColor = objectRenderer.material.color;
        }
        else
        {
            Debug.Log(gameObject.name + " has no renderer, highlight won't work.");
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
        if (canOnlyUseOnce && hasBeenUsed) return;

        hasBeenUsed = true;

        if (isDrink)
        {
            StartCoroutine(DrinkSequence());
        }
        else
        {
            Debug.Log("Interacted with " + gameObject.name);
        }
    }

    private IEnumerator DrinkSequence()
    {
        Debug.Log("Drinking...");

        yield return new WaitForSeconds(drinkDelay);

        if (DrunkManager.instance != null)
        {
            DrunkManager.instance.StartDrunkMode();
        }

        if (drinkVisual != null)
        {
            drinkVisual.SetActive(false);
        }
        else
        {
            gameObject.SetActive(false);
        }

        Debug.Log("Now drunk. Everything is going downhill.");
    }
}