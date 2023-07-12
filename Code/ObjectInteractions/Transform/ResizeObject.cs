using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeObject : MonoBehaviour
{
    Vector3 initialScale;
    Vector3 finalScale;
    public float timeToReach;
    public float finalScaleMultiplier;
    float distanceBetweenScales;
    bool locked;

    private void Start()
    {
        initialScale = transform.localScale;
        finalScale = initialScale*finalScaleMultiplier;
        distanceBetweenScales = Vector3.Distance(initialScale, finalScale);
    }
    public void Scale()
    {
        if(locked) return;
        StopAllCoroutines();
        StartCoroutine(ScaleCoroutine());
    }
    public void ResetScale()
    {
        if(locked) return;
        StopAllCoroutines();
        StartCoroutine(ResetCoroutine());
    }
    IEnumerator ScaleCoroutine()
    {
        float distanceToScale = Vector3.Distance(transform.localScale, finalScale);
        float time = distanceToScale / distanceBetweenScales * timeToReach;
        Vector3 _initScale = transform.localScale;
        float timer = 0f;
        while (timer<time)
        {
            transform.localScale = Vector3.Lerp(_initScale, finalScale, timer / time);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.localScale = finalScale;
    }
    IEnumerator ResetCoroutine()
    {
        float distanceToScale = Vector3.Distance(transform.localScale, initialScale);
        float time = distanceToScale / distanceBetweenScales * timeToReach;
        Vector3 _initScale = transform.localScale;
        float timer = 0f;
        while (timer<time)
        {
            transform.localScale = Vector3.Lerp(_initScale, initialScale, timer / time);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.localScale = initialScale;

    }
    
    public void SetLocked(bool state)
    {
        locked = state;
    }
}
