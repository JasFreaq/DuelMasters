using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class CivilizationConditionParam : EffectTargetingConditionParameter, ICardIntrinsicParam
{
    #region Helper Data Structure(s)

    [System.Serializable]
    private class CivilizationCondition
    {
        public bool non = false;
        public CardParams.Civilization[] civilization;
        public ConnectorType connector;
    }

    #endregion

    [SerializeReference] private List<CivilizationCondition> _civilizationConditions = new List<CivilizationCondition>();

    public override bool IsConditionSatisfied(CardInstance cardInstToCheck)
    {
        bool result = true;
        CardData cardData = cardInstToCheck.CardData;

        if (_civilizationConditions.Count > 0)
        {
            for (int i = 0, n = _civilizationConditions.Count; i < n; i++)
            {
                CivilizationCondition civilizationCondition = _civilizationConditions[i];

                bool res0 = cardData.Civilization.SequenceEqual(civilizationCondition.civilization);
                if (civilizationCondition.non)
                    res0 = !res0;

                result = result && res0;

                if (n > 1 && i < n - 1)
                {
                    if (civilizationCondition.connector == ConnectorType.And && !result)
                        break;
                    if (civilizationCondition.connector == ConnectorType.Or && result)
                        break;
                }
            }
        }

        return result;
    }

#if UNITY_EDITOR

    public override void DrawParamInspector()
    {
        if (_civilizationConditions.Count > 0)
        {
            List<CivilizationCondition> removedConditions = new List<CivilizationCondition>();
            GUILayout.Label("Civilizations:", EditorStatics.EffectTargetingConditionParamLabelStyle);

            for (int i = 0, n = _civilizationConditions.Count; i < n; i++)
            {
                CivilizationCondition civilizationCondition = _civilizationConditions[i];
                CardParams.Civilization[] civilization = civilizationCondition.civilization;

                GUILayout.BeginHorizontal();
                civilizationCondition.non = GUILayout.Toggle(civilizationCondition.non, "Non");

                ReorderableList list = new ReorderableList(civilization,
                    typeof(CardParams.Civilization), true, true, true, false);
                list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    rect.y += 2;
                    Rect elementRect = new Rect(rect.x, rect.y, 120, EditorGUIUtility.singleLineHeight);
                    civilization[index] = (CardParams.Civilization)EditorGUI.EnumPopup(elementRect, civilization[index]);
                };
                list.drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, "Civilization");
                };
                list.onAddCallback = reorderableList =>
                {
                    List<CardParams.Civilization> tempCivilization = new List<CardParams.Civilization>(civilization);
                    tempCivilization.Add(0);
                    civilization = tempCivilization.ToArray();
                };
                list.DoLayoutList();

                civilizationCondition.civilization = civilization;
                GUILayout.EndHorizontal();

                if (GUILayout.Button("Remove Civilization"))
                {
                    removedConditions.Add(civilizationCondition);
                }

                if (n > 1 && i < n - 1)
                    civilizationCondition.connector = EditorUtils.DrawFoldout(civilizationCondition.connector);
            }

            foreach (CivilizationCondition civilizationCondition in removedConditions)
            {
                _civilizationConditions.Remove(civilizationCondition);
            }
        }

        if (GUILayout.Button("Add Civilization"))
        {
            CardParams.Civilization[] civilization = new CardParams.Civilization[1];
            CivilizationCondition civilizationCondition = new CivilizationCondition
            {
                civilization = civilization
            };
            _civilizationConditions.Add(civilizationCondition);
        }
    }

    public override bool IsAssignedValue()
    {
        return _civilizationConditions.Count > 0;
    }

    public override string GetEditorRepresentationString()
    {
        return $"\nCivilization is{ToString()}";
    }

#endif

    public override string GetGameRepresentationString()
    {
        return ToString() + " card";
    }
    
    public override string ToString()
    {
        string str = "";
        for (int i = 0, n = _civilizationConditions.Count; i < n; i++)
        {
            CivilizationCondition civilizationCondition = _civilizationConditions[i];
            if (civilizationCondition.non)
                str += $" non-{CardParams.StringFromCivilization(civilizationCondition.civilization)}";
            else
                str += $" {CardParams.StringFromCivilization(civilizationCondition.civilization)}";

            if (n > 1 && i < n - 1)
                str += $" {_civilizationConditions[i].connector} ";
        }

        return str;
    }
}
