using UnityEngine;

[CreateAssetMenu(fileName = "DataConfig", menuName = "DataConfig", order = 1)]
public class DataConfig : ScriptableObject
{
    [SerializeField] private string fileName;
    [SerializeField] private bool useEncryption;

    public string GetFileName()
    {
        return fileName;
    }
    public bool GetEncrypt()
    {
        return useEncryption;
    }
}