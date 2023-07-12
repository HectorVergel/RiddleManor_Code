using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    void Update()
    {
        Vector3 camPos = Camera.main.transform.position;
        transform.LookAt(camPos);
        transform.Rotate(new Vector3(90,180,180));
    }
}
