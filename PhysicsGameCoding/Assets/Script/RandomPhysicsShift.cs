using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomPhysicsBurst : MonoBehaviour
{
    public float minTime = 5f;
    public float maxTime = 10f;
    public float burstForce = 8f;

    public List<Rigidbody> physicsObjects = new List<Rigidbody>();

    void Start()
    {
        StartCoroutine(BurstRoutine());
    }

    IEnumerator BurstRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minTime, maxTime));

            if (DrunkManager.instance != null && DrunkManager.instance.isDrunk && physicsObjects.Count > 0)
            {
                Rigidbody rb = physicsObjects[Random.Range(0, physicsObjects.Count)];
                if (rb != null)
                {
                    Vector3 randomForce = new Vector3(
                        Random.Range(-1f, 1f),
                        Random.Range(0.5f, 1.5f),
                        Random.Range(-1f, 1f)
                    ).normalized * burstForce;

                    rb.AddForce(randomForce, ForceMode.Impulse);
                }
            }
        }
    }
}