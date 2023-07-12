using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RoomTrigger : MonoBehaviour
{
    public UnityEvent onRoomChanged;
    public List<TriggerEvent> events;
    public BoxCollider trigger;
    public BoxCollider cameraBox;
    public float extraHeight;
    public float extraDepth;
    public Transform spawnPoint;
    public int roomID;
    public bool saveTrigger;
    bool triggered;
    public void RoomOnTriggerEnter(Collider other)
    {
        if(other.tag == "CharacterController")
        {
            if(other.GetComponent<BookGhost>()!=null)
            {
                CameraController.instance.ChangeRoom(cameraBox, extraHeight, extraDepth);
            }
            else 
            {
                PlayerController.instance.lastRoomTriggerPlayer = this;
                ChangeRoom();
            }
        }
    }

    private void DoEvents()
    {
        foreach (TriggerEvent triggerEvent in events)
        {
            triggerEvent.DoMyEvent();
        }
    }
    public void ChangeRoom()
    {
        if(saveTrigger && !triggered) Save();
        DoEvents();
        CameraController.instance.ChangeRoom(cameraBox, extraHeight, extraDepth);
        onRoomChanged?.Invoke();
    }

    private void Save()
    {
        triggered = true;
        Book.instance.ResetBookGraphics();
        GameSaveManager.instance.SetCurrentRoom(roomID);
        GameSaveManager.instance.EnableLevels();
        GameSaveManager.instance.UnenableLevels();
        UIRoomNames.instance.ShowText(roomID);
    }
}



