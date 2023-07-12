using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Interactable : MonoBehaviour
{
    public Transform iconDisplayPos;
    public Transform textDisplayPos;
    public DialogueNode startNode;
    private bool playerIn;
    public UnityEvent OnEnd;
    private void Update()
    {
        if (PlayerController.instance.CanInteract() && playerIn)
        {
            WorldScreenUI.instance.SetIcon(IconType.Dialogue, PlayerController.instance.transform.position+new Vector3(0,1,0));
        }
        else
        {
            WorldScreenUI.instance.HideIcon(IconType.Dialogue);
        }
    }

    private void DialogueInteract(InputAction.CallbackContext context)
    {
        Debug.Log("A");
        if (context.performed && PlayerController.instance.CanInteract())
        {
            DialogueDisplay.instance.onEndEvent = OnEnd;
            WorldScreenUI.instance.SetDialogue(startNode, textDisplayPos, null);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "CharacterController") return;

        if (other.tag == "Player" && !playerIn)
        {
            InputManager.GetAction("Push").action += DialogueInteract;
            playerIn = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "CharacterController") return;

        if (other.tag == "Player" && playerIn)
        {
            InputManager.GetAction("Push").action -= DialogueInteract;
            playerIn=false;
        }
    }
}





