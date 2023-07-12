
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Globalization;
using System.Reflection;

public class DynamicData
{
    private Dictionary<string, FieldInfo> fields = new Dictionary<string, FieldInfo>();

    private bool initialised = false;
    private void Initialize()
    {
        // Reflection-based discovery of public fields
        FieldInfo[] allFields = this.GetType().GetFields();
        foreach (FieldInfo field in allFields)
        {
            string name = field.Name.ToUpper();
            fields.Add(name, field);
        }
        initialised = true;
    }
    public T Get<T>(string name)
    {
        if (!initialised) Initialize();
        object value = null;

        name = name.ToUpper();
        if (fields.ContainsKey(name)) // name refers to a field 
            value = fields[name].GetValue(this);
        else
            Debug.LogWarning("Unknown key: " + name);
        return (T)value;
    }

    public void Put(string name, object value)
    {
        if (!initialised) Initialize();
        name = name.ToUpper();
        if (fields.ContainsKey(name)) // name refers to a field 
            fields[name].SetValue(this, value);
    }
}

