using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class EffectFunction
{
    private int _value;
    
    public void SetValue(int newValue)
    {
        _value = newValue;
    }

    public void SetValue<T>(T newValue) where T : Enum
    {
        _value = Convert.ToInt32(newValue);
    }
    
    public int GetValue()
    {
        return _value;
    }

    public T GetValue<T>() where T : Enum
    {
        return (T) Enum.Parse(typeof(T), _value.ToString());
    }

    public override string ToString()
    {
        return _value.ToString();
    }
}
