using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Cinematic : MonoBehaviour
{
    public float delay;
    public Transform target;
    public float time;
    public bool oneShot;
    bool activated;
    public Cinematic nextCinematic;
    public UnityEvent OnCinematicEnd;
    public void PlayCinematic()
    {
        if(oneShot && activated) return;
        activated = true;
        StartCoroutine(Delay());
    }
    IEnumerator Delay()
    {
        PlayerController.instance.BlockPlayerInputs(false);
        yield return new WaitForSeconds(delay);
        InstantCinematic();
    }
    public void InstantCinematic()
    {
        CameraController.instance.Cinematic(this,target==null?transform:target,time);
        StartCoroutine(WaitForCinematicEnd());
    }
    IEnumerator WaitForCinematicEnd()
    {
        yield return new WaitForSeconds(time);
        OnCinematicEnd?.Invoke();
    }
}
