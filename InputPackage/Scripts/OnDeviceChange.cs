using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnDeviceChange : MonoBehaviour
{
    public UnityEvent onKeyboard;
    public UnityEvent onGamepad;
    private void OnEnable() {
        InputManager.OnDeviceChanged += OnChange;
        OnChange(InputManager.device);
    }
    private void OnDisable() {
        InputManager.OnDeviceChanged -= OnChange;
    }

    void OnChange(Devices newDevice)
    {
        switch (newDevice)
        {
            case Devices.Keyboard:
            onKeyboard?.Invoke();
            break;

            case Devices.Gamepad:
            onGamepad?.Invoke();
            break;
        }
    }
}
