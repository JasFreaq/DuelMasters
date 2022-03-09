using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.DuelMasters;
using UnityEngine;

namespace DuelMasters.Card.Data.Effects.Functionality.Parameters
{
    #region Helper Data Structures

    [System.Serializable]
    public class MovementZones
    {
        public CardZoneType fromZone, toZone;
        public bool showSearchedCard;

        public NumericParamsHolder numericParams = new NumericParamsHolder();
    }

    #endregion

    public class RegionMovementFuncParam : MultipliableFuncParam
    {
        private MovementZones _movementZones = new MovementZones();

#if UNITY_EDITOR

        public override IReadOnlyList<EffectFunctionalityType> FunctionalityTypes
        {
            get
            {
                return new List<EffectFunctionalityType>()
                {
                    EffectFunctionalityType.RegionMovement
                };
            }
        }
#endif

        public MovementZones MovementZones
        {
            get { return _movementZones; }
        }

#if UNITY_EDITOR

        public override void DrawInspector()
        {
            GUILayout.Label("From");
            _movementZones.fromZone = EditorUtils.DrawFoldout(_movementZones.fromZone);

            if (_movementZones.fromZone == CardZoneType.Deck)
                _movementZones.showSearchedCard = GUILayout.Toggle(_movementZones.showSearchedCard, "Show Card");
            
            _movementZones.numericParams.DrawInspector();

            GUILayout.Label("To");
            _movementZones.toZone = EditorUtils.DrawFoldout(_movementZones.toZone);
        }

        public override bool ShouldAssignCriterion()
        {
            return false;
        }

#endif
    }
}