using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Data : DynamicData
{
    //AUDIO
    public float volumeMaster;
    public float volumeMusic;
    public float volumeSFX;
    public float volumeVoice;
    //OPTIONS
    public bool cameraShake;
    public bool cursorLock;
    public bool fullScreen;
    public bool subtitles;
    public bool vSync;
    public int resolution;
    public int defaultResolution;
    //LOCALIZATION
    public int language;
    //PLAYER
    public int roomID;

    public Data()
    {
        //AUDIO
        AudioDefaultConfig defaultAudio =  Resources.Load<AudioDefaultConfig>("AudioDefaultConfig");
        volumeMaster = defaultAudio.Master;
        volumeMusic = defaultAudio.Music;
        volumeSFX = defaultAudio.SFX;
        volumeVoice = defaultAudio.Voice;
        //OPTIONS
        OptionsDefaultConfig defaultOptions =  Resources.Load<OptionsDefaultConfig>("OptionsDefaultConfig");
        cameraShake = defaultOptions.cameraShake;
        cursorLock = defaultOptions.cursorLock;
        fullScreen = defaultOptions.fullScreen;
        subtitles = defaultOptions.subtitles;
        vSync = defaultOptions.vSync;
        resolution = defaultOptions.resolution;
        defaultResolution = defaultOptions.resolution;
        //LOCALIZATION
        language = 0;
        roomID = 0;
        

    }
}

