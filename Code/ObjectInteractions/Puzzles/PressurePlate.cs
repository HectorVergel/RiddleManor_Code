using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
    public UnityEvent OnPressed;
    public UnityEvent OnUnpressed;
    List<GameObject> onTop = new List<GameObject>();
    bool locked;
    Animator anim;
    private void Start() {
        anim = GetComponentInChildren<Animator>();
        OnPressed.AddListener(()=> anim.SetBool("Pressed",true));
        OnUnpressed.AddListener(()=> anim.SetBool("Pressed",false));
    }
    public bool IsPressed()
    {
        return onTop.Count > 0;
    }
    private void OnTriggerEnter(Collider other) {
        if(locked) return;
        if (other.tag == "CharacterController") return;
        if(onTop.Count == 0) 
        {
            AudioManager.Play("pressurePlate").Volume(0.3f).Pitch(1+UnityEngine.Random.Range(0.1f,0.2f));
            OnPressed?.Invoke();
        }
        onTop.Add(other.gameObject);
    }
    private void OnTriggerExit(Collider other) {
        if(locked) return;
        if (onTop.Contains(other.gameObject))
        {
            onTop.Remove(other.gameObject);
            if(onTop.Count == 0) OnUnpressed?.Invoke();
        }
    }
    public void SetLocked(bool state)
    {
        locked = state;
    }
}

