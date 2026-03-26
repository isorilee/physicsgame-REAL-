using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    //the color of object glows when the player looks at it 
    public Color highlightColor = new Color(r: 1f, g: 0.95f, b: 0.6f);
    //how strong the highlight color blends with the original color 0= no effect 1=full replace 
    [Range(0, 1f)] public float highlightStrength = 0.4f;

    private Renderer objectRenderer; //the render comp on this obj 
    private Color originalColor; //the color before any highlight was applied 
    private bool isHighlighted = false; //are we currently highlighted? 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        objectRenderer = GetComponent<Renderer>(); //cache the renderer so we are not calling every frame 
        if (objectRenderer != null)
        {
            //store the original color so we can restore it after unhighlighting 
            //we read from the materials color property 
            //we use sharedmaterial to read the base color without instancing 

            originalColor = objectRenderer.material.color;
        }

        else
        {
            Debug.Log($"InteractableObject object on {gameObject.name} has no renderer, highlighter won't work");

        }

    }

    // Update is called once per frame
    public void Highlight()
    {
        if (isHighlighted || objectRenderer == null)
        {
            Debug.Log("no obj renderer & ishighlighted is true");
            return; 
        }

        //color.lerp blends bent the original color and the highlighted color 
        //by the highlight strength amt 
        //we use material not shared material to create a uniqye instance here 
        //so we dont effect every obj using the same material 

        objectRenderer.material.color = Color.Lerp(originalColor, highlightColor, highlightStrength);
        isHighlighted = true;
    }
    //called by object grabber when the player looks away
    //restores original color 

    public void Unhighlight()
    {
        if (!isHighlighted || objectRenderer == null) return; 
        objectRenderer.material.color = originalColor;
        isHighlighted = false;


    }



}
