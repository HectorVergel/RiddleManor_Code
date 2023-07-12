using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

/** This class allows to serialize dictionary atributes */
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    // Creating two list to save the key and the value
    [SerializeField] private List<TKey> keys = new List<TKey> ();
    [SerializeField] private List<TValue> values = new List<TValue> ();
    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();

        foreach(KeyValuePair<TKey, TValue> pair in this)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }


    public void OnAfterDeserialize()
    {
        this.Clear ();

        if (keys.Count != values.Count)
        {
            Debug.LogWarning("Tried to deserialize a SerializableDictionary, but the amount of keys ( " + keys.Count + " ) does not match the number of " +
                "values (" + values.Count + " ) which indicates that something went wrong");
        }

        for (int i = 0; i < keys.Count; i++)
        {
            this.Add(keys[i], values[i]); 
        }
    }

}
