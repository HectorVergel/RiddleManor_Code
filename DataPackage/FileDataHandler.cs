using System.IO;
using System;
using UnityEngine;

public class FileDataHandler
{
    private string dataDirPath = "";
    private string dataFileName = "";
    private bool useEncryption = false;
    private readonly string encryptionCodeWord = "alexby";
    public FileDataHandler(string dataDirPath, string dataFileName, bool useEncryption)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
        this.useEncryption = useEncryption;
    }

    public Data Load()
    {
        string _fullPath = Path.Combine(dataDirPath, dataFileName );
        Data _loadedData = null;

        try
        {
            string _dataToLoad = "";

            using (FileStream _stream = new FileStream(_fullPath, FileMode.Open))
            {
                using (StreamReader _reader = new StreamReader(_stream))
                {
                    _dataToLoad = _reader.ReadToEnd();
                }
            }
            
            if (useEncryption)
            {
                _dataToLoad = EncryptDecrypt(_dataToLoad);
            }

            _loadedData = JsonUtility.FromJson<Data>(_dataToLoad);
        }
        catch (Exception e)
        {

            Debug.LogWarning("Error occured when trying to load data from file: " + _fullPath + "\n" + e);
        }
        return _loadedData;
    }

    public void Save(Data data)
    {
        string _fullPath = Path.Combine(dataDirPath, dataFileName);

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_fullPath));
            string _dataToStore = JsonUtility.ToJson(data, true);


            if (useEncryption)
            {
                _dataToStore = EncryptDecrypt(_dataToStore);
            }

            using (FileStream _stream  = new FileStream(_fullPath, FileMode.Create))
            {
                using (StreamWriter _writer = new StreamWriter(_stream))
                {
                    _writer.Write(_dataToStore);
                }
            }
        }
        catch(Exception e)
        {
            Debug.LogWarning("Error occured when trying to save data to file: " + _fullPath + "\n" + e);
        }
    }

    //XOR encryption algorithm to protect the data saved
    private string EncryptDecrypt(string data)
    {
        string _modifiedData = "";
        for (int i = 0; i < data.Length; i++)
        {
            _modifiedData += (char)(data[i] ^ encryptionCodeWord[i % encryptionCodeWord.Length]);
        }
        return _modifiedData;
    }
}