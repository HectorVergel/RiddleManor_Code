using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DialogueEventHandler : MonoBehaviour
{
    public DialogueNode startNode;
    public Transform otherTransform;
    public float dialogueStartDelay;
    public Animation actorAnimation;
    public AnimationClip actorStartAnimation;
    public AnimationClip actorEndAnimation;
    public GameObject interactableParticles;
    public UnityEvent OnFinish;
    public void StartDialogueEvent()
    {
        StartCoroutine(StartDialogueCoroutine());
    }

    IEnumerator StartDialogueCoroutine()
    {
        if (actorStartAnimation != null) actorAnimation.PlayQueued(actorStartAnimation.name);
        if(Book.instance != null) Book.instance.ResetBookGraphics();
        if(interactableParticles!=null) interactableParticles.SetActive(false);
        if (Book.instance != null) BookMovement.instance.DialogueStarted();
        yield return new WaitForSeconds(dialogueStartDelay);
        WorldScreenUI.instance.SetDialogue(startNode, otherTransform, this);
        
    }

    public void DisableInteractParticles()
    {
        if (interactableParticles != null) interactableParticles.SetActive(false);
    }

    public void EnableInteractParticles()
    {
        if (interactableParticles != null) interactableParticles.SetActive(true);
    }
    public void DoEndAnimation()
    {
        OnFinish?.Invoke();
        if (actorEndAnimation != null) actorAnimation.PlayQueued(actorEndAnimation.name);
    }



}

