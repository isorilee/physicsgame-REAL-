using UnityEngine;
using System.Collections;

public class AnnoyingInteractable : MonoBehaviour
{
    // how many times it can dodge before behaving normally
    public int maxDodges = 3;

    // how far it moves each time
    public float dodgeDistance = 0.5f;

    // how fast it moves
    public float dodgeSpeed = 5f;

    private int dodgeCount = 0;
    private bool isMoving = false;

    // call this instead of normal interaction
    public void TryInteract(Transform player)
    {
        // if we still have dodges left, move instead of interacting
        if (dodgeCount < maxDodges && !isMoving)
        {
            dodgeCount++;
            StartCoroutine(Dodge(player));
        }
        else
        {
            // finally allow interaction
            Debug.Log("ok fine you got me :(");
        }
    }

    IEnumerator Dodge(Transform player)
    {
        isMoving = true;

        Vector3 startPos = transform.position;

        // move AWAY from the player
        Vector3 dir = (transform.position - player.position).normalized;

        // add a little randomness so it's not perfectly predictable
        dir += new Vector3(
            Random.Range(-0.3f, 0.3f),
            0,
            Random.Range(-0.3f, 0.3f)
        );

        Vector3 targetPos = startPos + dir.normalized * dodgeDistance;

        float t = 0;

        while (t < 1f)
        {
            t += Time.deltaTime * dodgeSpeed;

            // smooth movement
            transform.position = Vector3.Lerp(startPos, targetPos, t);

            yield return null;
        }

        isMoving = false;
    }
}