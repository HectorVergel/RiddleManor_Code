using UnityEngine;
using UnityEngine.Audio;

public class Sound
{
    public AudioClip sound;
    public AudioMixerGroup group;
    public Sound(AudioClip _sound, AudioMixerGroup _group)
    {
        sound = _sound;
        group = _group;

    }
}
