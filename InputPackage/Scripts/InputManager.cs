using System.Collections.Generic;
using System;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.SceneManagement;

//InputAction.CallbackContext context


static class InputManager
{
    static GameObject sceneInput;
    static List<ActionContainer> events = new List<ActionContainer>(); //List of all action events
    static List<ActionContainer> temporaryDisabledEvents = new List<ActionContainer>();
    public delegate void DeviceChanged(Devices currentDevice);
    public static event DeviceChanged OnDeviceChanged;
    static PlayerInput playerInput; //Current PlayerInput
    static EventSystem eventSystem; //Current EventSystem
    public static Devices device { get; private set; }
    static string path; //Path of the InputManager prefab
    static bool cleanListeners = true;
    static string currentActionMap;
    static InputManager()
    {
        //Constructor called only one time when a static method of this class is called
        path = "InputManager"; //Modify path how you want, but remember the root file is Resources
        //Checks if prefab is there
        if(Resources.Load(path) == null)
        {
            Debug.LogWarning("Prefab not found. Your input prefab have to be at " + path);
            return;
        }
        //Instantiates InputManager prefab, sets the current EventSystem and PlayerInput, and creates all events for each action it has
        if(sceneInput!=null) return;
        sceneInput = CreateInputOnScene();
        MonoBehaviour.DontDestroyOnLoad(sceneInput);
        playerInput = sceneInput.GetComponent<PlayerInput>();
        currentActionMap = playerInput.defaultActionMap;
        if(playerInput.currentControlScheme == "Gamepad") device = Devices.Gamepad;
        else device = Devices.Keyboard;
        eventSystem = sceneInput.GetComponent<EventSystem>();
        CreateEvents(playerInput);
        //Subscribe to scene changes and device changes
        SceneManager.sceneUnloaded += ClearListeners;
        InputUser.onChange += OnInputDeviceChange;
    }
    public static void ClearListeners(Scene a)
    {
        if(!cleanListeners) return;
        //Removes all subscribed listeners of all events
        foreach (ActionContainer _event in events)
        {
            _event.ClearListeners();
        }
        //OnDeviceChanged = null;
    }
    public static void OnInputDeviceChange(InputUser user, InputUserChange change, InputDevice _device)
    {
       if(change == InputUserChange.ControlSchemeChanged)
       {
            if(user.controlScheme.Value.name == "Gamepad") device = Devices.Gamepad;
            else device = Devices.Keyboard;
            OnDeviceChanged?.Invoke(device);
       }
       //Do stuff when device changed
    }
    public static void CreateEvents(PlayerInput playerInput)
    {
        //Creates one event for each action in PlayerInput
        events = new List<ActionContainer>();

        foreach (InputActionMap actionMap in playerInput.actions.actionMaps)
        {
            foreach (InputAction act in actionMap)
            {
                ActionContainer newActionEvent = new ActionContainer(act);
                events.Add(newActionEvent);
            }
        }
    }
    static GameObject CreateInputOnScene()
    {
        //Creates the InputManager GameObject on Scene, wich works alongside with InputManager(this)
        return MonoBehaviour.Instantiate(Resources.Load(path) as GameObject,Vector3.zero,Quaternion.identity);
    }
    public static ActionContainer GetAction(string _actionName)
    {
        //Subscribe to event of action named _actionName with _method
        return GetEvent(_actionName);
    }
    public static void DisableAllActions()
    {
        foreach (ActionContainer action in events)
        {
            if(action.GetEnabled())
            {
                temporaryDisabledEvents.Add(action);
                action.SetEnabled(false);
            }
        }
    }
    public static void EnableAllActions()
    {
        foreach (ActionContainer action in temporaryDisabledEvents)
        {
            action.SetEnabled(true);
        }
        temporaryDisabledEvents = new List<ActionContainer>();
    }
    public static void EnableAllActionsIndependently()
    {
        foreach (ActionContainer action in events)
        {
            action.SetEnabled(true);
        }
        temporaryDisabledEvents = new List<ActionContainer>();
    }
    public static void ActionEnabled(string _actionName,bool _enabled)
    {
        //Sets event enabled of action named _ActionName to _enabled
        GetEvent(_actionName)?.SetEnabled(_enabled);
    }
    public static void ActionsEnabled(string[] _actionNames,bool _enabled)
    {
        //Sets events enabled of actions named _ActionName to _enabled
        foreach (string _actionName in _actionNames)
        {
            ActionEnabled(_actionName,_enabled);
        }
    }
    public static void ChangeActionMap(string _actionMapName)
    {
        //Change PlayerInput Action Map to one named _actionMapName
        if(playerInput!=null)
        {
            playerInput.SwitchCurrentActionMap(_actionMapName);
            currentActionMap = _actionMapName;
        }
    }
    static ActionContainer GetEvent(string _actionName)
    {
        //Returns event of action named _actionName
        foreach (ActionContainer _event in events)
        {
            if(_event.actionName == _actionName)
            {
                return _event;
            }
        }
        if(playerInput!=null) Debug.LogWarning("Action named " + _actionName + " doesn't exist");
        return null;
    }

    public static string GetCurrentActionMap()
    {
        return currentActionMap;
    }
    public static void Init(){}
}
public enum Devices
{
    Keyboard,
    Gamepad
}
