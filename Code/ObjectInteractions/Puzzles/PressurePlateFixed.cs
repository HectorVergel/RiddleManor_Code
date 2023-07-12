using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PressurePlateFixed : MonoBehaviour
{
    public int numberLinked;
    public UnityEvent OnPressed;
    public UnityEvent OnUnpressed;
    List<GameObject> onTop = new List<GameObject>();
    bool locked;
    Animator anim;
    bool pressed;
    private void Start() {
        anim = GetComponentInChildren<Animator>();
    }
    public bool IsPressed()
    {
        return pressed;
    }
    private void OnTriggerEnter(Collider other) {
        if(locked) return;
        if (other.tag == "CharacterController") return;
        if(onTop.Contains(other.gameObject))
        {
            StopAllCoroutines();
            return;
        }
        if (onTop.Count == 0)
        {
            anim.SetBool("Pressed",true);
            AudioManager.Play("pressurePlate").Volume(0.3f).Pitch(1+UnityEngine.Random.Range(0.1f,0.2f));
        }
        onTop.Add(other.gameObject);
        FixedPlateObject objectPressing = other.GetComponent<FixedPlateObject>();
        if(objectPressing!=null)
        {
            if (objectPressing.number == numberLinked)
            {
                pressed = true;
                OnPressed?.Invoke();
            }
        }
    }
    private void OnTriggerExit(Collider other) {
        if(locked) return;
        if (other.tag == "CharacterController") return;
        StartCoroutine(RemoveObject(other));
    }
    public void SetLocked(bool state)
    {
        locked = state;
    }
    public void SetAnimPressed(bool state)
    {
        anim.SetBool("Pressed",state);
    }
    IEnumerator RemoveObject(Collider other)
    {
        yield return new WaitForEndOfFrame();
        if (onTop.Contains(other.gameObject))
        {
            onTop.Remove(other.gameObject);
            if (onTop.Count == 0)
            {
                anim.SetBool("Pressed", false);
                AudioManager.Play("pressurePlate").Volume(0.3f).Pitch(1 + UnityEngine.Random.Range(0.1f, 0.2f));
            } 
        }
        FixedPlateObject objectPressing = other.GetComponent<FixedPlateObject>();
        if(objectPressing!=null)
        {

            if (objectPressing.number == numberLinked)
            {
                OnUnpressed?.Invoke();
                pressed = false;
            }
        }
    }
}

