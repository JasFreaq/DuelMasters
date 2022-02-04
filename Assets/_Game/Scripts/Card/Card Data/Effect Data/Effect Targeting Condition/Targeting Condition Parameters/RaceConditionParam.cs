using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RaceConditionParam : EffectTargetingConditionParameter, ICardIntrinsicParam
{
    #region Helper Data Structure(s)

    [System.Serializable]
    public class RaceCondition
    {
        public bool non = false;
        public CardParams.Race race;
        public ConnectorType connector;
    }

    #endregion

    [SerializeReference] private List<RaceCondition> _raceConditions = new List<RaceCondition>();
    
    public override bool IsConditionSatisfied(CardInstance cardInstToCheck)
    {
        bool result = true;
        CardData cardData = cardInstToCheck.CardData;

        if (_raceConditions.Count > 0)
        {
            if (cardData is CreatureData creatureData)
            {
                for (int i = 0, n = _raceConditions.Count; i < n; i++)
                {
                    RaceCondition raceCondition = _raceConditions[i];

                    bool res0 = false;
                    foreach (CardParams.Race race in creatureData.Race)
                    {
                        if (race == raceCondition.race)
                        {
                            res0 = true;
                            break;
                        }

                        bool breakLoop = false;
                        switch (race)
                        {
                            case CardParams.Race.ArmoredDragon:
                            case CardParams.Race.EarthDragon:
                            case CardParams.Race.VolcanoDragon:
                            case CardParams.Race.ZombieDragon:
                                switch (raceCondition.race)
                                {
                                    case CardParams.Race.ArmoredDragon:
                                    case CardParams.Race.EarthDragon:
                                    case CardParams.Race.VolcanoDragon:
                                    case CardParams.Race.ZombieDragon:
                                        res0 = true;
                                        breakLoop = true;
                                        break;
                                }
                                break;
                        }

                        if (breakLoop)
                            break;
                    }

                    if (raceCondition.non)
                        res0 = !res0;

                    result = result && res0;

                    if (n > 1 && i < n - 1)
                    {
                        if (raceCondition.connector == ConnectorType.And && !result)
                            break;
                        if (raceCondition.connector == ConnectorType.Or && result)
                            break;
                    }
                }
            }
            else
                result = false;
        }

        return result;
    }
    
#if UNITY_EDITOR
    
    public override void DrawParamInspector()
    {
        if (_raceConditions.Count > 0)
        {
            List<RaceCondition> removedConditions = new List<RaceCondition>();
            GUILayout.Label("Races:", EditorStatics.EffectTargetingConditionParamLabelStyle);

            for (int i = 0, n = _raceConditions.Count; i < n; i++)
            {
                RaceCondition raceCondition = _raceConditions[i];

                GUILayout.BeginHorizontal();

                raceCondition.non = GUILayout.Toggle(raceCondition.non, "Non");
                raceCondition.race = EditorUtils.DrawFoldout(raceCondition.race, 1);
                if (n > 1 && i < n - 1)
                    raceCondition.connector = EditorUtils.DrawFoldout(raceCondition.connector);

                GUILayout.EndHorizontal();

                if (GUILayout.Button("Remove Race"))
                {
                    EditorGUILayout.Space(5);
                    removedConditions.Add(raceCondition);
                }
            }

            foreach (RaceCondition raceCondition in removedConditions)
            {
                _raceConditions.Remove(raceCondition);
            }
        }

        if (GUILayout.Button("Add Race"))
        {
            RaceCondition raceCondition = new RaceCondition();
            _raceConditions.Add(raceCondition);
        }
    }

    public override bool IsAssignedValue()
    {
        return _raceConditions.Count > 0;
    }

    public override string GetEditorRepresentationString()
    {
        return $"\nRace is{ToString()}";
    }

#endif

    public override string GetGameRepresentationString()
    {
        return ToString();
    }

    public override string ToString()
    {
        string str = "";
        for (int i = 0, n = _raceConditions.Count; i < n; i++)
        {
            RaceCondition raceCondition = _raceConditions[i];
            if (raceCondition.non)
                str += $" non-{CardParams.StringFromRace(raceCondition.race)}";
            else
                str += $" {CardParams.StringFromRace(raceCondition.race)}";

            if (n > 1 && i < n - 1)
                str += $" {_raceConditions[i].connector} ";
        }

        return str;
    }
}
