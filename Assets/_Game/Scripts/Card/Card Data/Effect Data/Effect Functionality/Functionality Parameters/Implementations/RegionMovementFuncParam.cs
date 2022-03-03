using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.DuelMasters;
using UnityEngine;

namespace DuelMasters.Card.Data.Effects.Functionality.Parameters
{
    #region Helper Data Structures

    [System.Serializable]
    public enum DeckCardMoveType
    {
        Top,
        SearchShuffle
    }

    [System.Serializable]
    public class MovementZones
    {
        public CardZoneType fromZone, toZone;
        public DeckCardMoveType deckCardMove;
        public bool showSearchedCard;

        public CountQuantifierType countQuantifier;
        public int moveCount = 1;
    }

    #endregion

    public class RegionMovementFuncParam : EffectFunctionalityParameter
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
            {
                _movementZones.deckCardMove = EditorUtils.DrawFoldout(_movementZones.deckCardMove);
                if (_movementZones.deckCardMove == DeckCardMoveType.SearchShuffle)
                    _movementZones.showSearchedCard = GUILayout.Toggle(_movementZones.showSearchedCard, "Show Card");
            }

            if (_movementZones.moveCount > 1)
                _movementZones.countQuantifier = EditorUtils.DrawFoldout(_movementZones.countQuantifier);
            if (int.TryParse(EditorGUILayout.TextField($"{_movementZones.moveCount}"), out int num))
                _movementZones.moveCount = num;
            //DrawMultiplyVal();

            GUILayout.Label("To");
            _movementZones.toZone = EditorUtils.DrawFoldout(_movementZones.toZone);

            if (_movementZones.toZone == CardZoneType.Deck)
                _movementZones.deckCardMove = EditorUtils.DrawFoldout(_movementZones.deckCardMove);
        }

        public override bool ShouldAssignCriterion()
        {
            throw new System.NotImplementedException();
        }

#endif
    }
}