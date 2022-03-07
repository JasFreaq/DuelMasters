using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.DuelMasters;
using UnityEngine;

namespace DuelMasters.Card.Data.Effects.Functionality.Parameters
{
    #region Helper Data Structures

    [System.Serializable]
    public class RaceHolder
    {
        public CardParams.Race race;
    }

    #endregion

    public class KeywordFuncParam : EffectFunctionalityParameter
    {
        private KeywordType _keyword;
        private List<RaceHolder> _vortexRaces = new List<RaceHolder>();

#if UNITY_EDITOR

        public override IReadOnlyList<EffectFunctionalityType> FunctionalityTypes
        {
            get
            {
                return new List<EffectFunctionalityType>()
                {
                    EffectFunctionalityType.Keyword
                };
            }
        }
#endif

        public KeywordType Keyword
        {
            get { return _keyword; }
        }

        public List<RaceHolder> VortexRaces
        {
            get { return _vortexRaces; }
        }

#if UNITY_EDITOR

        public override void DrawInspector()
        {
            _keyword = EditorUtils.DrawFoldout(_keyword);
            if (_keyword == KeywordType.VortexEvolution)
                DrawVortexRaces();
        }

        private void DrawVortexRaces()
        {
            GUILayout.BeginVertical();

            if (_vortexRaces.Count > 0)
            {
                List<RaceHolder> removedConditions = new List<RaceHolder>();
                GUILayout.Label("Races:");

                for (int i = 0, n = _vortexRaces.Count; i < n; i++)
                {
                    RaceHolder raceHolder = _vortexRaces[i];

                    GUILayout.BeginHorizontal();

                    raceHolder.race = EditorUtils.DrawFoldout(raceHolder.race, 1);

                    GUILayout.EndHorizontal();

                    if (GUILayout.Button("Remove Race"))
                    {
                        EditorGUILayout.Space(5);
                        removedConditions.Add(raceHolder);
                    }
                }

                foreach (RaceHolder raceHolder in removedConditions)
                {
                    _vortexRaces.Remove(raceHolder);
                }
            }

            if (GUILayout.Button("Add Race"))
            {
                RaceHolder raceHolder = new RaceHolder();
                _vortexRaces.Add(raceHolder);
            }

            GUILayout.EndVertical();
        }

        public override bool ShouldAssignCriterion()
        {
            return true;
        }

#endif
    }
}