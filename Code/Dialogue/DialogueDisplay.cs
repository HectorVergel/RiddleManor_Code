using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class DialogueDisplay : MonoBehaviour
{
    [SerializeField] public GameObject dialogueRender;
    [SerializeField] public GameObject uiRenderer;
    [SerializeField] TextMeshProUGUI emisorName;
    [SerializeField] TextMeshProUGUI dialogueText;
    private DialogueNode startNode;
    private Transform interactablePos;
    private DialogueNode currentNode;
    [SerializeField] float defaultTypeSpeed;
    [SerializeField] float fastTypeSpeed;
    private float currentTypeSpeed;
    [SerializeField] string dialogueID;
    public bool isTextFinished;
    private bool onAnimation;
    private DialogueEventHandler currentEventHandler;
    public UnityEvent onEndEvent;
    private DIALOGUE_STATE currentState = DIALOGUE_STATE.DEFAULT;
    public static DialogueDisplay instance;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else Destroy(this);

        dialogueRender.SetActive(false);

    }

    private void OnEnable()
    {
        InputManager.GetAction("Push").action += Interact;
        InputManager.GetAction("ExitDialogue").action += End;
    }

    private void OnDisable()
    {
        InputManager.GetAction("Push").action -= Interact;
        InputManager.GetAction("ExitDialogue").action -= End;
    }



    private void Interact(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (isTextFinished)
            {
                currentTypeSpeed = defaultTypeSpeed;
                currentState = DIALOGUE_STATE.DEFAULT;
                NextSentence();
            }
            else if (!onAnimation)
            {
                switch (currentState)
                {
                    case DIALOGUE_STATE.DEFAULT:
                        currentTypeSpeed = fastTypeSpeed;
                        currentState = DIALOGUE_STATE.FAST;
                        break;
                    case DIALOGUE_STATE.FAST:
                        ShowFullText();
                        currentState = DIALOGUE_STATE.SKIP;
                        break;
                }
            }
        }
    }
    private void End(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            EndDialogue();
        }
    }

    private void SetEmisorName()
    {
        emisorName.color = currentNode.nameColor;
        emisorName.text = LocalizationManager.GetLocalizedValue(currentNode.emisorID);
    }
    public void StartDialogue()
    {
        currentEventHandler.DisableInteractParticles();
        StartCoroutine(WaitToLand());
        if (PlayerController.instance != null) PlayerController.instance.BlockPlayerInputs(false);
        if (PlayerController.instance != null) PlayerController.instance.GetAnimator().SetBool("isMoving", false);
        dialogueRender.SetActive(true);
        dialogueText.text = "";
        currentNode = startNode;
        currentState = DIALOGUE_STATE.DEFAULT;
        currentTypeSpeed = defaultTypeSpeed;
        SetEmisorName();
        StartCoroutine(Type());
    }
    IEnumerator WaitToLand()
    {
        if (PlayerController.instance != null)
        {

            while (PlayerController.instance.GetIsJumping())
            {
                yield return null;
            }
            PlayerController.instance.characterController.enabled = false;
        }
    }
    public void SetStartNode(DialogueNode startNode)
    {
        this.startNode = startNode;
    }

    public void SetEventHandler(DialogueEventHandler handler)
    {
        this.currentEventHandler = handler;
    }
    public void SetInteractablePos(Transform pos)
    {
        interactablePos = pos;
    }


    IEnumerator Type()
    {
        onAnimation = false;
        isTextFinished = false;
        foreach (char letter in LocalizationManager.GetLocalizedValue(currentNode.textID).ToCharArray())
        {

            dialogueText.text += letter;
            AudioManager.Play("dialogue" + UnityEngine.Random.Range(1, 12).ToString()).Volume(1f);
            yield return new WaitForSeconds(currentTypeSpeed);
        }
        isTextFinished = true;

    }


    private void NextSentence()
    {
        if (currentNode.TargetNode != null)
        {
            currentNode = currentNode.TargetNode;
            SetEmisorName();
            dialogueText.text = "";
            StartCoroutine(Type());
        }
        else
        {
            EndDialogue();
        }
    }
    private void EndDialogue()
    {
        if (BookMovement.instance != null) BookMovement.instance.DialogueEnded();
        if (PlayerController.instance != null) PlayerController.instance.BlockPlayerInputs(true);
        if (PlayerController.instance != null) PlayerController.instance.characterController.enabled = true;
        onEndEvent?.Invoke();
        currentEventHandler?.DoEndAnimation();
        dialogueRender.gameObject.SetActive(false);
        this.enabled = false;
    }

    private void ShowFullText()
    {
        StopAllCoroutines();
        isTextFinished = true;
        dialogueText.text = LocalizationManager.GetLocalizedValue(currentNode.textID);
    }
}


public enum DIALOGUE_STATE
{
    DEFAULT,
    FAST,
    SKIP
}

