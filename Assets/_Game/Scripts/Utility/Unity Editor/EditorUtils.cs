using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityEditor.DuelMasters
{
    public static class EditorUtils
    {
        public static T DrawFoldout<T>(T currentValue, int enumIndexAdjustment = 0) where T : Enum
        {
            Type enumType = typeof(T);
            int currentInt = (int)Enum.Parse(enumType, currentValue.ToString());
            int newInt = EditorGUILayout.Popup(currentInt - enumIndexAdjustment, Enum.GetNames(enumType));
            if (currentInt != newInt)
                newInt += enumIndexAdjustment;
            return (T)Enum.Parse(enumType, newInt.ToString());
        }
    }
}