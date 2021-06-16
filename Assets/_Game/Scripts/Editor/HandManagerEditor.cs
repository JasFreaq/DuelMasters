using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HandManager))]
public class HandManagerEditor : Editor
{
    private SerializedProperty _flippedIntermediateHolder;
    private SerializedProperty _previewTargetPosition;
    private SerializedProperty _previewTargetRotation;
    private SerializedProperty _previewTargetScale;

    private void OnEnable()
    {
        _flippedIntermediateHolder = serializedObject.FindProperty("_flippedIntermediateHolder");
        _previewTargetPosition = serializedObject.FindProperty("_previewTargetPosition");
        _previewTargetRotation = serializedObject.FindProperty("_previewTargetRotation");
        _previewTargetScale = serializedObject.FindProperty("_previewTargetScale");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();

        HandManager hand = (HandManager) target;
        if (hand.IsPlayer)
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_previewTargetPosition);
            EditorGUILayout.PropertyField(_previewTargetRotation);
            EditorGUILayout.PropertyField(_previewTargetScale);
        }
        else
        {
            EditorGUILayout.PropertyField(_flippedIntermediateHolder);
        }
        
        serializedObject.ApplyModifiedProperties();
    }
}
