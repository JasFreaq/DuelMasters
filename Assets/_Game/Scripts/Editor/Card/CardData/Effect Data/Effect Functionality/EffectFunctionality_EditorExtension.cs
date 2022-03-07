using System.Collections;
using System.Collections.Generic;
using DuelMasters.Editor.Data.DataSetters;
using UnityEngine;

namespace DuelMasters.Editor.Data.Extensions
{
    public static class EffectFunctionality_EditorExtension
    {
        public static void AssignConditionParam(this EffectFunctionality functionality)
        {
            EffectFunctionality_ParamSettingWrapper.AssignFunctionalityParam(functionality);
        }

        public static string GetEditorRepresentationString(this EffectFunctionality functionality)
        {
            string str = "";//GetTypeRepresentation();

            //if (_targetType == CardTargetType.TargetOther || _shouldMultiplyVal)
            //    str += $" {_targetingCriterion}";

            //if (_targetingCondition != null)
            //    str += $" where{_targetingCondition}";

            //if (_connector != ConnectorType.None)
            //    str += $" {_connector}";

            //if (_subFunctionality)
            //    str += $"\n\t{_subFunctionality}";

            return str;

            #region Local Functions

            //string GetTypeRepresentation()
            //{
            //    switch (_type)
            //    {
            //        case EffectFunctionalityType.RegionMovement:
            //            return GetRegionMovementString();

            //        case EffectFunctionalityType.AttackTarget:
            //            return _attackType.ToString();

            //        case EffectFunctionalityType.TargetBehaviour:
            //            return _targetBehaviour.ToString();

            //        case EffectFunctionalityType.Keyword:
            //            return _keyword.ToString();

            //        case EffectFunctionalityType.MultipleBreaker:
            //            return _multipleBreaker.ToString();

            //        case EffectFunctionalityType.ToggleTap:
            //            return _tapState.ToString();

            //        case EffectFunctionalityType.Destroy:
            //            string str3 = "Destroy";
            //            if (_targetType == CardTargetType.TargetSelf)
            //                str3 += " self";
            //            return str3;

            //        case EffectFunctionalityType.Discard:
            //            return $"{_targetType} {_discardParam}";

            //        case EffectFunctionalityType.LookAtRegion:
            //            return $"{_lookAtParam} in {_targetType}'s {_lookAtParam.lookAtZone}";

            //        case EffectFunctionalityType.PowerAttacker:
            //            return $"Power Attacker +{_powerBoost}";

            //        case EffectFunctionalityType.GrantPower:
            //            return $"Gets +{_powerBoost}";

            //        case EffectFunctionalityType.GrantFunction:
            //            string str2 = "Grant Function";
            //            if (_alterFunctionUntilEndOfTurn)
            //                str2 += " until the end of turn";
            //            return str2;

            //        case EffectFunctionalityType.CostAdjustment:
            //            return $"Adjust cost by {_costAdjustmentAmount}";

            //        default:
            //            return _type.ToString();
            //    }
            //}

            //string GetRegionMovementString()
            //{
            //    string str1 = "";

            //    switch (_targetPlayer)
            //    {
            //        case PlayerTargetType.Player:
            //            str1 = "Player moves ";
            //            break;
            //        case PlayerTargetType.Opponent:
            //            str1 = "Opponent moves ";
            //            break;
            //    }

            //    str1 += $"{_movementZones.moveCount} card ";
            //    if (_movementZones.moveCount > 1)
            //        str1 += "s ";

            //    if (_movementZones.fromZone == CardZoneType.Deck)
            //    {
            //        if (_movementZones.deckCardMove == DeckCardMoveType.Top)
            //            return $"Draw {_movementZones.countQuantifier} {_movementZones.moveCount}";

            //        str1 += $"after searching deck to {_movementZones.toZone}";
            //        if (_movementZones.showSearchedCard)
            //            str1 += " and show it to the opponent";
            //    }
            //    else if (_movementZones.toZone == CardZoneType.Deck)
            //    {
            //        str1 += $"from {_movementZones.fromZone} ";
            //        if (_movementZones.deckCardMove == DeckCardMoveType.Top)
            //            str1 += "to top of Deck";
            //        else
            //            str1 += "and shuffle into Deck";
            //    }
            //    else
            //        str1 += $"from {_movementZones.fromZone} to {_movementZones.toZone}";
            //    return str1;
            //}

            #endregion
        }
    }
}