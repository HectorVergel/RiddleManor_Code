using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class CopyMovement : MonoBehaviour
{
    [SerializeField] private Transform elementToCopy;
    [SerializeField] private bool rotationInZ;
    [SerializeField] private bool copyObjectRotation;
    private bool locked;
    public bool isPlayer;
    public bool noInverse;


    private void Awake()
    {

        if (rotationInZ) transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, -transform.localScale.z);
        else transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y, transform.localScale.z);
        SetElementInMirrorPosition();
        locked = true;


    }

    public void SetLocked(bool state)
    {
        locked = state;
    }
    private void Update()
    {
        if (isPlayer && locked) return;
        UpdatePosition();
        if (copyObjectRotation) UpdateRotation();


    }

    private void SetElementInMirrorPosition()
    {
        Quaternion reflectedRotation = Quaternion.Euler(elementToCopy.eulerAngles.x, -elementToCopy.eulerAngles.y, elementToCopy.eulerAngles.z);

        transform.rotation = reflectedRotation;
    }


    private void UpdateRotation()
    {
        SetElementInMirrorPosition();
    }
    private void UpdatePosition()
    {

        float distanceToReference = Mathf.Abs(MirrorPuzzleManager.instance.planeReference.position.z - elementToCopy.position.z);
        Vector3 newPosition = Vector3.zero;

        newPosition.x = elementToCopy.position.x;
        newPosition.z = (elementToCopy.position.z + (2 * distanceToReference));
        newPosition.y = elementToCopy.position.y;

        transform.position = newPosition;
    }

}
