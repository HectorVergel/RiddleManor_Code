using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CoroutineManager
{
    static CoroutineHolder sceneObject;
    static CoroutineManager()
    {
        sceneObject = CreateCoroutineHolder().GetComponent<CoroutineHolder>();
    }
    public static void StartCoroutine(IEnumerator coroutine)
    {
        sceneObject.StartCoroutine(coroutine);
    }
    static GameObject CreateCoroutineHolder()
    {
        GameObject holder = MonoBehaviour.Instantiate(new GameObject());
        holder.name = "COROUTINES HOLDER";
        holder.AddComponent<CoroutineHolder>();
        MonoBehaviour.DontDestroyOnLoad(holder);
        return holder;
    }
}
