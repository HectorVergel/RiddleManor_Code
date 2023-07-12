using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour
{
    public UnityEvent eventToDo;
    public UnityEvent eventToDoWithTime;
    public float timeToEventWithTimer;
    private bool activated;
    public bool needInput;
    private bool inCollider = false;
    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Player" && !activated)
        {
            inCollider = true;
            if (needInput) return;
            if (eventToDoWithTime != null)
            {
                StartCoroutine(EventTimer());
            }
            activated = true;
            inCollider = false;
            WorldScreenUI.instance.HideIcon(IconType.Dialogue);
            eventToDo?.Invoke();
        }
    }

    IEnumerator EventTimer()
    {
        yield return new WaitForSeconds(timeToEventWithTimer);
        eventToDoWithTime?.Invoke();
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && !activated)
        {
            inCollider = false;
            WorldScreenUI.instance.HideIcon(IconType.Dialogue);
        }
    }
    private void Update()
    {
        if (!needInput) return;
        if(inCollider && !activated) WorldScreenUI.instance.SetIcon(IconType.Dialogue, PlayerController.instance.characterController.bounds.center + new Vector3(0, 1, 0));
        else if(inCollider) WorldScreenUI.instance.HideIcon(IconType.Dialogue);
        if (InputManager.GetAction("Push").context.WasPerformedThisFrame() && !activated && inCollider)
        {
            activated = true;
            inCollider = false;
            WorldScreenUI.instance.HideIcon(IconType.Dialogue);
            eventToDo?.Invoke();
            if(eventToDoWithTime != null)
            {
                StartCoroutine(EventTimer());
            }
           
        }
    }

    public void DoMyEvent()
    {
        if (eventToDoWithTime != null)
        {
            StartCoroutine(EventTimer());
        }
        activated = true;
        inCollider = false;
        WorldScreenUI.instance.HideIcon(IconType.Dialogue);
        eventToDo?.Invoke();
    }
}
