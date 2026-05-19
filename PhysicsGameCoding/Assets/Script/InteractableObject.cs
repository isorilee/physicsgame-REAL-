using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [Header("Interaction Type")]
    public bool isDrink = false;

    [Header("Object Settings")]
    public bool disappearAfterDrink = true;

    [Header("Highlight")]
    public Renderer[] renderersToHighlight;
    public Color highlightColor = Color.yellow;

    private Color[] originalColors;
    private bool isHighlighted = false;

    private void Start()
    {
        originalColors = new Color[renderersToHighlight.Length];

        for (int i = 0; i < renderersToHighlight.Length; i++)
        {
            if (renderersToHighlight[i] != null)
            {
                originalColors[i] = renderersToHighlight[i].material.color;
            }
        }
    }

    public void Interact()
    {
        Debug.Log("INTERACTED WITH: " + gameObject.name);

        if (isDrink)
        {
            FPSplayer player = FindFirstObjectByType<FPSplayer>();

            if (player != null)
            {
                player.StartDrinkStandSequence();
            }

            if (disappearAfterDrink)
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.Log("Interacted with non-drink object: " + gameObject.name);
        }
    }

    public void Highlight()
    {
        if (isHighlighted) return;

        isHighlighted = true;

        for (int i = 0; i < renderersToHighlight.Length; i++)
        {
            if (renderersToHighlight[i] != null)
            {
                renderersToHighlight[i].material.color = highlightColor;
            }
        }
    }

    public void Unhighlight()
    {
        if (!isHighlighted) return;

        isHighlighted = false;

        for (int i = 0; i < renderersToHighlight.Length; i++)
        {
            if (renderersToHighlight[i] != null)
            {
                renderersToHighlight[i].material.color = originalColors[i];
            }
        }
    }
}