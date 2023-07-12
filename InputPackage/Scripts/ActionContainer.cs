using UnityEngine.InputSystem;
using System;

public class ActionContainer
{
    //Base event class of action events
    public string actionName; //Action name
    bool _enabled; //If this event can be triggered
    //Specific events
    public event Action<InputAction.CallbackContext> action;
    public InputAction context;
    public ActionContainer(InputAction _action)
    {
        actionName = _action.name;
        _enabled = true;
        _action.started += InvokeEvent;
        _action.performed += InvokeEvent;
        _action.canceled += InvokeEvent;
        context = _action;
    }
    void InvokeEvent(InputAction.CallbackContext context)
    {
        //Invokes of this event and his specifics if input is enabled
        if(_enabled) action?.Invoke(context);
    }
    public void SetEnabled(bool enable)
    {
        //Sets if this event is enabled or not
        _enabled = enable;
    }
    public bool GetEnabled()
    {
        //Sets if this event is enabled or not
        return _enabled;
    }
    public void ClearListeners()
    {
        //Clears all listeners of this event and his specifics
        action = null;
    }
    public Action<InputAction.CallbackContext> GetAction()
    {
        return action;
    }
}
