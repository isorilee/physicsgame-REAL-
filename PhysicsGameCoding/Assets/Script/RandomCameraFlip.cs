using UnityEngine;
using System.Collections;

public class RandomCameraFlip : MonoBehaviour
{
    public float minTimeBetweenFlips = 8f;
    public float maxTimeBetweenFlips = 18f;
    public float flipDuration = 2.5f;

    private bool isFlipping = false;

    void Start()
    {
        StartCoroutine(FlipRoutine());
    }

    IEnumerator FlipRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minTimeBetweenFlips, maxTimeBetweenFlips));

            //if (DrunkManager.instance != null && DrunkManager.instance.isDrunk && !isFlipping)
            {
                yield return StartCoroutine(DoFlip());
            }
        }
    }

    IEnumerator DoFlip()
    {
        isFlipping = true;

        Quaternion startRot = transform.localRotation;
        Quaternion flippedRot = startRot * Quaternion.Euler(0f, 0f, 180f);

        float timer = 0f;
        while (timer < flipDuration)
        {
            timer += Time.deltaTime;
            float t = timer / flipDuration;
            transform.localRotation = Quaternion.Slerp(startRot, flippedRot, t);
            yield return null;
        }

        yield return new WaitForSeconds(1.5f);

        timer = 0f;
        while (timer < flipDuration)
        {
            timer += Time.deltaTime;
            float t = timer / flipDuration;
            transform.localRotation = Quaternion.Slerp(flippedRot, startRot, t);
            yield return null;
        }

        isFlipping = false;
    }
}