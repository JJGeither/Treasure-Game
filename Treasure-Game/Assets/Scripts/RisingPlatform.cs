using System.Collections;
using UnityEngine;

public class RisingPlatform : MonoBehaviour
{
    public float riseSpeed = 25f;  // Speed at which the platform rises
    private float riseDuration = 0.5f;  // Duration for which the platform rises

    void Start()
    {
        StartCoroutine(RiseForDuration());
    }

    private IEnumerator RiseForDuration()
    {
        float elapsedTime = 0f;

        while (elapsedTime < riseDuration)
        {
            transform.position += Vector3.up * riseSpeed * Time.deltaTime;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
