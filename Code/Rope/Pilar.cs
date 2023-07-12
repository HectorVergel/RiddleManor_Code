using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Pilar : MonoBehaviour
{
    public UnityEvent OnActivate;
    public UnityEvent OnDisable;
    public PilarManager pilar;
    int collisions;
    bool locked;
    public ColorPropertySetter propSet;
    public float intensity = 2f;
    private Renderer render;
    private void Start()
    {
        render = propSet.GetComponent<Renderer>();
    }
    public void OnCollision()
    {
        if(locked) return;
        if(collisions==0)
        {
            pilar.AddPilar(this);
            if(pilar.CorrectOrder(this)) propSet.SetIntensity(render.material, intensity);
            OnActivate?.Invoke();
            AudioManager.Play("tonguePilarContact").Volume(0.4f);
        }
        collisions++;
    }

    
    public void OnExitCollision()
    {
        if(locked) return;
        collisions--;
        if(collisions==0)
        {
            propSet.SetIntensity(render.material, -10);
            pilar.RemovePilar(this);
            OnDisable?.Invoke();
            AudioManager.Play("tonguePilarContact").Volume(0.4f).Pitch(0.7f);
        }
    }
    public void SetLocked(bool state)
    {
        locked = state;
    }
}
