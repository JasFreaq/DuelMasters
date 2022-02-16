using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MonoBehaviourExtension
{
    public static Coroutine<T> StartCoroutine<T>(this MonoBehaviour obj, IEnumerator coroutine)
    {
        Coroutine<T> coroutineObj = new Coroutine<T>();
        coroutineObj.coroutine = obj.StartCoroutine(coroutineObj.InternalRoutine(coroutine));
        return coroutineObj;
    }
}
