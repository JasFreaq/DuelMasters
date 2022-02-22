using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityEditor.DuelMasters
{
    public static class EditorUtils
    {
        public static T DrawFoldout<T>(T currentValue, out bool changed, int enumIndexAdjustment = 0) where T : Enum
        {
            Type enumType = typeof(T);
            int currentInt = (int)Enum.Parse(enumType, currentValue.ToString());
            int newInt = EditorGUILayout.Popup(currentInt - enumIndexAdjustment, Enum.GetNames(enumType));

            changed = currentInt != newInt;
            if (changed)
                newInt += enumIndexAdjustment;

            return (T)Enum.Parse(enumType, newInt.ToString());
        }
        
        public static T DrawFoldout<T>(T currentValue, int enumIndexAdjustment = 0) where T : Enum
        {
            return DrawFoldout(currentValue, out _, enumIndexAdjustment);
        }
    }
}