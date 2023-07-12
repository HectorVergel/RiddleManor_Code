using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioDefaultConfig", menuName = "Audio Default", order = 1)]
public class AudioDefaultConfig : ScriptableObject
{
    [Range(0f,1f)]
    public float Master = 0.5f;
    [Range(0f,1f)]
    public float Music = 1f;
    [Range(0f,1f)]
    public float SFX = 1f;
    [Range(0f,1f)]
    public float Voice = 1f;
    public Dictionary<string,float> GetDefaultVolumes()
    {
        Dictionary<string,float> defaultVolumes = new Dictionary<string, float>();
        defaultVolumes.Add(nameof(Master),Master);
        defaultVolumes.Add(nameof(Music),Music);
        defaultVolumes.Add(nameof(SFX),SFX);
        defaultVolumes.Add(nameof(Voice),Voice);
        
        return defaultVolumes;
    }
}