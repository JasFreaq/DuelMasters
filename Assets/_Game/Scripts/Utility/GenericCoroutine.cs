using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coroutine<T>
{
    public T returnVal;
    public Coroutine coroutine;

    public IEnumerator InternalRoutine(IEnumerator iterator)
    {
        while (true)
        {
            if (!iterator.MoveNext())
                yield break;

            object yielded = iterator.Current;
            
            if (yielded != null)
            {
                bool isMatchingType = false;

                Type yieldType = yielded.GetType();
                do
                {
                    if (yieldType == typeof(T))
                    {
                        isMatchingType = true;
                        break;
                    }

                    yieldType = yieldType.BaseType;
                } while (yieldType != null);

                if (isMatchingType) 
                {
                    returnVal = (T) yielded;
                    yield break;
                }
            }
            
            yield return iterator.Current;
        }
    }
}
