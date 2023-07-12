using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    Quaternion initialRotation;
    Quaternion finalRotation;
    public float timeToReach;
    [SerializeField] float delay;
    public Vector3 _finalRotation;
    float angleBetweenRotations;
    bool locked;

    private void Start()
    {
        initialRotation = transform.localRotation;
        finalRotation = Quaternion.Euler(_finalRotation);
        angleBetweenRotations = Quaternion.Angle(initialRotation,finalRotation);
    }
    public void Rotate()
    {
        if(locked) return;
        StopAllCoroutines();
        StartCoroutine(RotateCoroutine());
    }
    public void ResetRotation()
    {
        if(locked) return;
        StopAllCoroutines();
        StartCoroutine(ResetCoroutine());
    }
    IEnumerator RotateCoroutine()
    {
        yield return new WaitForSeconds(delay);
        float angleToRotation =  Quaternion.Angle(transform.localRotation, finalRotation);
        float time = angleToRotation / angleBetweenRotations * timeToReach;
        Quaternion _initRot = transform.localRotation;
        float timer = 0f;
        while (timer<time)
        {
            transform.localRotation = Quaternion.Lerp(_initRot,finalRotation,timer/time);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.localRotation = finalRotation;
    }
    IEnumerator ResetCoroutine()
    {
        float angleToRotation =  Quaternion.Angle(transform.localRotation, initialRotation);
        float time = angleToRotation / angleBetweenRotations * timeToReach;
        Quaternion _initRot = transform.localRotation;
        float timer = 0f;
        while (timer<time)
        {
            transform.localRotation = Quaternion.Lerp(_initRot,initialRotation,timer/time);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.localRotation = initialRotation;
    }
    public void SetFinalRotation()
    {
        transform.localRotation = finalRotation;
    }
    
    public void SetLocked(bool state)
    {
        locked = state;
    }
}
