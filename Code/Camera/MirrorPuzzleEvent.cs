using System.Collections;
using UnityEngine;

public class MirrorPuzzleEvent : MonoBehaviour
{
    public float shakeTime;
    public float timeToDialogue;
    DialogueEventHandler dialogueEvent;
    private bool activated;

    private void Start()
    {
        dialogueEvent = GetComponent<DialogueEventHandler>();
    }
    public void StartEvent()
    {
        StartCoroutine(PuzzleMirror());
    }

    IEnumerator PuzzleMirror()
    {
        CameraShake.instance.StartShake(shakeTime);
        yield return new WaitForSeconds(timeToDialogue);
        dialogueEvent.StartDialogueEvent();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && !activated)
        {
            
            activated = true;
            StartEvent();
        }
    }
}