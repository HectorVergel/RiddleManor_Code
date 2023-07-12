using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Init : MonoBehaviour
{
    void Start()
    {
        UIUtilities.Init();
        AudioManager.Init();
        InputManager.Init();
    }
}
