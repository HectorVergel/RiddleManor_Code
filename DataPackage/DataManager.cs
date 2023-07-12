using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Globalization;
using System.Reflection;
using UnityEngine.Events;

public static class DataManager 
{
    private static Data gameData;
    private static FileDataHandler dataHandler;
    public static Action onSave;
    
    static DataManager()
    {
        DataConfig config = (DataConfig)Resources.Load("DataConfig");
        dataHandler = new FileDataHandler(Application.persistentDataPath, config.GetFileName(), config.GetEncrypt());
        Application.quitting += SaveGame;
        LoadGame();
    }
    public static void NewGame()
    {
        gameData = new Data();
    }
    private static void LoadGame()
    {
        /** Load any data saved in the Data Handler */
        gameData = dataHandler.Load();

        if (gameData == null)
        {
            Debug.LogWarning("No game data was found. A new game is created");
            NewGame();
        }
    }
    private static void SaveGame()
    {
        if (gameData == null)
        {
            Debug.LogError("No game data was found. Can't save the game");
            return;
        }
        // Alert all to save their data
        onSave?.Invoke();
        /** We save the data into the data handler */
        dataHandler.Save(gameData); 
    }
    public static T Load<T>(string name)
    {
        return gameData.Get<T>(name);
    }
    public static void Save(string name, object value)
    {
        gameData.Put(name, value);
    }
    public static void Init(){}
}
