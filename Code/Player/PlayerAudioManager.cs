using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerAudioManager : MonoBehaviour
{
    public bool isNotPlayer;
    public void PlayStep()
    {
        if (isNotPlayer) return;
        AudioManager.Play("kidStep" + Random.Range(1, 10).ToString()).Volume(0.1f);
    }

    public void PlayLand()
    {
        if (isNotPlayer) return;
        AudioManager.Play("kidJumpLanding" + Random.Range(1, 3).ToString()).Volume(0.1f);
    }

    public void PlayJump()
    {
        if (isNotPlayer) return;
        AudioManager.Play("kidJumpEfffort" + Random.Range(1, 5).ToString()).Volume(0.1f);
    }
}
