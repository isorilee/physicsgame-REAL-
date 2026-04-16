using UnityEngine;
using System.Collections;

public class RandomGravityShift : MonoBehaviour
{
    public float minTime = 7f;
    public float maxTime = 14f;

    public float gravityStrength = 9.81f;
    public float sideGravityAmount = 4f;

    private Vector3 normalGravity;

    void Start()
    {
        normalGravity = new Vector3(0f, -gravityStrength, 0f);
        Physics.gravity = normalGravity;
        StartCoroutine(GravityRoutine());
    }

    IEnumerator GravityRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minTime, maxTime));

            if (DrunkManager.instance != null && DrunkManager.instance.isDrunk)
            {
                Vector3 weirdGravity = new Vector3(
                    Random.Range(-sideGravityAmount, sideGravityAmount),
                    -gravityStrength,
                    Random.Range(-sideGravityAmount, sideGravityAmount)
                );

                Physics.gravity = weirdGravity;

                yield return new WaitForSeconds(2.5f);

                Physics.gravity = normalGravity;
            }
        }
    }

    void OnDisable()
    {
        Physics.gravity = new Vector3(0f, -gravityStrength, 0f);
    }
}