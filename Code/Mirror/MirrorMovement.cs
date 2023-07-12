using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorMovement : MonoBehaviour
{
    Camera myCamera;
    Transform target;
    public Transform mirror;
    public Transform wall;
    public float offsetX;
    public float offsetNearPlane;

    void Start()
    {
        myCamera = GetComponent<Camera>();
        target = Camera.main.transform;
    }
    void Update()
    {

        /*Vector3 localTarget = mirror.InverseTransformPoint(target.position - new Vector3(0, offsetX, 0));
        Vector3 lookAtMirror = mirror.TransformPoint(new Vector3(-localTarget.x, localTarget.y, localTarget.z));
        transform.LookAt(lookAtMirror);*/

        Vector3 l_WorldPosition = target.transform.position;
        Vector3 l_LocalPosition = mirror.InverseTransformPoint(l_WorldPosition);
        transform.position = mirror.transform.TransformPoint(new Vector3(l_LocalPosition.x, l_LocalPosition.y, -l_LocalPosition.z));

        Vector3 l_WorldDirection = target.transform.forward;
        Vector3 l_LocalDirection = mirror.InverseTransformDirection(l_WorldDirection);
        Vector3 desiredForward = mirror.transform.TransformDirection(new Vector3(l_LocalDirection.x, l_LocalDirection.y, l_LocalDirection.z));
        transform.forward = new Vector3(desiredForward.x, desiredForward.y, -desiredForward.z);
        

        float l_Distance = Vector3.Distance(transform.position, mirror.transform.position);
        myCamera.nearClipPlane = l_Distance + offsetNearPlane;
        

    }

    
}
