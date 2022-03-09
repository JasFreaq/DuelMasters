using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.DuelMasters;
using UnityEngine;

[System.Serializable]
public class NumericParamsHolder
{
    private CountRangeType _countRangeType;
    private CountQuantifierType _countQuantifier;
    private int _count;

    public NumericParamsHolder() { }

    public NumericParamsHolder(NumericParamsHolder numericParams)
    {
        _countRangeType = numericParams.CountRangeType;
        _countQuantifier = numericParams.CountQuantifier;
        _count = numericParams.Count;
    }

    public NumericParamsHolder(CountRangeType countRangeType, CountQuantifierType countQuantifier, int count)
    {
        _countRangeType = countRangeType;
        _countQuantifier = countQuantifier;
        _count = count;
    }

    public CountRangeType CountRangeType
    {
        get { return _countRangeType; }
    }

    public CountQuantifierType CountQuantifier
    {
        get { return _countQuantifier; }
    }

    public int Count
    {
        get { return _count; }
    }

#if UNITY_EDITOR

    public void DrawInspector()
    {
        _countRangeType = EditorUtils.DrawFoldout(_countRangeType);
        if (_countRangeType == CountRangeType.Number)
        {
            _countQuantifier = EditorUtils.DrawFoldout(_countQuantifier);
            if (int.TryParse(EditorGUILayout.TextField($"{_count}"), out int num))
                _count = num;
        }
    }

#endif
}
