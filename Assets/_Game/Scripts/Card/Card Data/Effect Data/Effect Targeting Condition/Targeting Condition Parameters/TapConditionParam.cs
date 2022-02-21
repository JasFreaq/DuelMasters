using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.DuelMasters;
using UnityEngine;

namespace DuelMasters.Card.Data.Effects.TargetingCondition.Parameters
{
    public class TapConditionParam : SingleEnumConditionParam, IFieldStateParam
    {
        [SerializeReference] private TapStateType _tapCondition;

        public override bool IsConditionSatisfied(CardInstance cardInstToCheck)
        {
            bool result = true;

            if (_tapCondition == TapStateType.Tap)
                result = result && cardInstToCheck.IsTapped;
            else if (_tapCondition == TapStateType.Untap)
                result = result && !cardInstToCheck.IsTapped;

            return result;
        }

#if UNITY_EDITOR

        public override void DrawInspector()
        {
            GUILayout.BeginHorizontal();

            if (_assignedParameter)
            {
                GUILayout.Label("Tap State:", EditorStatics.EffectTargetingConditionParamLabelStyle);
                _tapCondition = EditorUtils.DrawFoldout(_tapCondition);

                if (GUILayout.Button("Remove Tap State"))
                    _assignedParameter = false;
            }
            else if (GUILayout.Button("Add Tap State"))
                _assignedParameter = true;

            GUILayout.EndHorizontal();
        }

        public override string GetEditorRepresentationString()
        {
            return $"\nTap state is {ToString()}";
        }

#endif

        public override string GetGameRepresentationString()
        {
            return $"\nTap state is {ToString()}";
        }

        public override string ToString()
        {
            string str = "";

            if (_tapCondition == TapStateType.Tap)
                str += "tapped";
            else if (_tapCondition == TapStateType.Untap)
                str += "untapped";

            return str;
        }
    }
}