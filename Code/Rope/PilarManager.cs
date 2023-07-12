using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PilarManager : MonoBehaviour
{
    public UnityEvent OnComplete;
    public UnityEvent OnUncomplete;
    public List<Pilar> correctPilarsOrder;
    List<Pilar> currentPilars = new List<Pilar>();
    bool completed;
    public void AddPilar(Pilar pilar)
    {
        currentPilars.Add(pilar);
        if(!completed) CheckPilars();
    }
    public void RemovePilar(Pilar pilar)
    {
        currentPilars.Remove(pilar);
        if(completed) CheckPilars();
    }
    void CheckPilars()
    {
        if(CorrectOrder()) OnComplete?.Invoke();
        else OnUncomplete?.Invoke();
        completed = CorrectOrder();
    }
    bool CorrectOrder()
    {
        if(currentPilars.Count < correctPilarsOrder.Count) return false;
        for (int i = 0; i < currentPilars.Count; i++)
        {
            if(currentPilars[i] != correctPilarsOrder[i]) return false;
        }
        return true;
    }
    public bool CorrectOrder(Pilar pilar)
    {
        return currentPilars[currentPilars.Count-1] == correctPilarsOrder[currentPilars.Count-1];
    }
}
