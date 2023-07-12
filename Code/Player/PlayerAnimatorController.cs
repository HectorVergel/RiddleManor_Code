using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    public List<Animator> playerAnimators = new List<Animator>();
    
    public void PlayRandomIdle()
    {
        foreach (Animator animator in playerAnimators)
        {
            animator.SetTrigger("Idle" + Random.Range(1, 3).ToString());
        }
    }
    public void SetFloat(string name, float value)
    {

        playerAnimators[0].SetFloat(name, value);
        playerAnimators[1].SetFloat(name, value);
        
    }

    public void SetBool(string name,bool state)
    {
        foreach (Animator animator in playerAnimators)
        {
            if(animator.Equals(null)) return;
            animator.SetBool(name, state);
        }
    }

    public void SetTrigger(string name)
    {
        foreach (Animator animator in playerAnimators)
        {
            animator.SetTrigger(name);
        }
    }

    

}
