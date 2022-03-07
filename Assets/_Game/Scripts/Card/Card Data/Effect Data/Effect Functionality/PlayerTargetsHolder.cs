using System.Collections;
using System.Collections.Generic;
using UnityEditor.DuelMasters;
using UnityEngine;

[System.Serializable]
public class PlayerTargetsHolder
{
    public PlayerTargetType choosingPlayer;
    public PlayerTargetType targetPlayer;

#if UNITY_EDITOR
    public void DrawInspector()
    {
        GUILayout.Label("Chooser:");
        choosingPlayer = EditorUtils.DrawFoldout(choosingPlayer);
        GUILayout.Label("Target:");
        targetPlayer = EditorUtils.DrawFoldout(targetPlayer);
    }
#endif
}