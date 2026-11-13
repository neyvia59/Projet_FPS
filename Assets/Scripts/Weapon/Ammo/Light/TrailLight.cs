using DG.Tweening;
using System.Collections;
using UnityEngine;

public class TrailLight : MonoBehaviour
{
    public float TimeBeforeFading = 2f;
    public float TimeBeforeDestroy = 3f;

    void Start()
    {
        StartCoroutine(FadingDelay());
        StartCoroutine(DestroyDelay());
    }

    private IEnumerator FadingDelay()
    {
        yield return new WaitForSeconds(TimeBeforeFading);
        GetComponent<Light>().DOIntensity(0, TimeBeforeDestroy - TimeBeforeFading);
    }

    private IEnumerator DestroyDelay()
    {
        yield return new WaitForSeconds(TimeBeforeDestroy);
        Destroy(gameObject);
    }
}
