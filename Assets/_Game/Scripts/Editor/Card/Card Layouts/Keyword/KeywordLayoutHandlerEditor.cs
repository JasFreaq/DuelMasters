using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(KeywordLayoutHandler))]
public class KeywordLayoutHandlerEditor : Editor
{
    private SerializedProperty _subDescHolder;

    private void OnEnable()
    {
        _subDescHolder = serializedObject.FindProperty("subDescHolder");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();

        KeywordLayoutHandler keywordLayout = (KeywordLayoutHandler) target;
        if (keywordLayout.IncludesSubDesc)
            EditorGUILayout.PropertyField(_subDescHolder);

        serializedObject.ApplyModifiedProperties();
    }
}
