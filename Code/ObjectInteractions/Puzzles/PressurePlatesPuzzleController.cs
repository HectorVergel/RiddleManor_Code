using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class PressurePlatesPuzzleController: MonoBehaviour
{
    [SerializeField] List<PressurePlateFixed> plates;
    [SerializeField] UnityEvent eventWhenComplete;
    private bool eventInvoked = false;
    public ChangeEmission triangle;
    public ChangeEmission box;
    public ChangeEmission circle;
    [ColorUsage(true, true)]
    public Color minIntensity;
    [ColorUsage(true, true)]
    public Color maxIntensity;

    private void Start() {
        SetBox(false);
        SetCircle(false);
        SetTriangle(false);
    }
    public void CheckIfComplete()
    {
        foreach (PressurePlateFixed plate in plates)
        {
            if (!plate.IsPressed()) return;
        }
        if (!eventInvoked)
        {
            eventWhenComplete?.Invoke();
            SetBox(true);
            SetCircle(true);
            SetTriangle(true);
            eventInvoked = true;
        } 
    }
    public void SetTriangle(bool pressed)
    {
        if(eventInvoked) return;
        if(pressed) triangle.SetEmission(maxIntensity);
        else triangle.SetEmission(minIntensity);
    }
    public void SetBox(bool pressed)
    {
        if(eventInvoked) return;
        if(pressed) box.SetEmission(maxIntensity);
        else box.SetEmission(minIntensity);
    }
    public void SetCircle(bool pressed)
    {
        if(eventInvoked) return;
        if(pressed) circle.SetEmission(maxIntensity);
        else circle.SetEmission(minIntensity);
    }

}

