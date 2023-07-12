using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public RotateObject rightDoor;
    public RotateObject leftDoor;
    public bool opened;
    private void Start() {
        if(opened)
        {
            rightDoor.SetFinalRotation();
            leftDoor.SetFinalRotation();
        }
    }
    public void Open()
    {
        rightDoor.Rotate();
        leftDoor.Rotate();
    }
    public void Close()
    {
        rightDoor.ResetRotation();
        leftDoor.ResetRotation();
    }
}
