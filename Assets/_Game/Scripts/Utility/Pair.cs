using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Pair<T>
{
    public T first;
    public T second;

    public Pair(T first, T second)
    {
        this.first = first;
        this.second = second;
    }

    public bool ValuesAreEqual()
    {
        return first.Equals(second);
    }
}
