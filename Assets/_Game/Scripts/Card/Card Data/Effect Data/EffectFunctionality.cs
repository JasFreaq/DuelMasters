using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Helper Data Structures

[System.Serializable]
public enum DeckCardMoveType
{
    Top,
    SearchShuffle
}

[System.Serializable]
public class MovementRegions
{
    public RegionType fromRegion, toRegion;
    public DeckCardMoveType deckCardMove;
    public bool showSearchedCard;

    public CountChoiceType countChoice;
    public int moveCount = 1;
}

[System.Serializable]
public class DiscardParam
{
    public DiscardType discardType;
    public CountType countType;
    public int discardCount = 1;

    public override string ToString()
    {
        string str = "discards ";
        if (countType == CountType.All)
            str += "all cards";
        else
        {
            str += $"{discardCount} card";
            if (discardCount > 1)
                str += "s ";

            switch (discardType)
            {
                case DiscardType.Random:
                    str += "at random";
                    break;
                case DiscardType.PlayerChoose:
                    str += "chosen by player";
                    break;
                case DiscardType.OpponentChoose:
                    str += "chosen by opponent";
                    break;
            }
        }

        return str;
    }
}

[System.Serializable]
public class LookAtParam
{
    public RegionType lookAtRegion;
    public CountType countType;
    public int lookCount = 1;

    public override string ToString()
    {
        string str = "Look at ";
        if (countType == CountType.All)
            str += "all cards";
        else
        {
            str += $"{lookCount} card";
            if (lookCount > 1)
                str += "s ";
        }

        return str;
    }
}

#endregion

public class EffectFunctionality : ScriptableObject
{
    [SerializeReference] private EffectFunctionalityType _type;
    [SerializeReference] private FunctionTargetType _target;
    [SerializeReference] private bool _assignedCondition, _connectSubFunctionality;
    [SerializeReference] private EffectTargetingParameter _targetingParameter = new EffectTargetingParameter();
    [SerializeReference] private EffectTargetingCondition _targetingCondition = new EffectTargetingCondition();
    [SerializeReference] private ConnectorType _connector;
    [SerializeReference] private EffectFunctionality _subFunctionality;

    #region Type Specific Members

    [SerializeReference] private MovementRegions _movementRegions = new MovementRegions();
    [SerializeReference] private AttackType _attackType;
    [SerializeReference] private TargetBehaviourType _targetBehaviour;
    [SerializeReference] private KeywordType _keyword;
    [SerializeReference] private MultipleBreakerType _multipleBreaker;
    [SerializeReference] private TapStateType _tapState;
    [SerializeReference] private DiscardParam _discardParam = new DiscardParam();
    [SerializeReference] private LookAtParam _lookAtParam = new LookAtParam();
    [SerializeReference] private int _attackBoost;
    [SerializeReference] private int _costAdjustmentAmount;

    [SerializeReference] private bool _shouldMultiplyVal = false;
    [SerializeReference] private bool _alterFunctionUntilEndOfTurn = true;
    
    #endregion

    public EffectFunctionalityType Type
    {
        get { return _type; }

#if UNITY_EDITOR
        set { _type = value; }
#endif
    }

    public FunctionTargetType Target
    {
        get { return _target; }

#if UNITY_EDITOR
        set { _target = value; }
#endif
    }

    public bool AssignedCondition
    {
        get { return _assignedCondition; }
#if UNITY_EDITOR
        set { _assignedCondition = value; }
#endif
    }

    public bool ConnectSubFunctionality
    {
        get { return _connectSubFunctionality; }

#if UNITY_EDITOR
        set { _connectSubFunctionality = value; }
#endif
    }

    public EffectTargetingParameter TargetingParameter
    {
        get { return _targetingParameter; }

#if UNITY_EDITOR
        set { _targetingParameter = value; }
#endif
    }
    
    public EffectTargetingCondition TargetingCondition
    {
        get { return _targetingCondition; }

#if UNITY_EDITOR
        set { _targetingCondition = value; }
#endif
    }

    public ConnectorType Connector
    {
        get { return _connector; }

#if UNITY_EDITOR
        set { _connector = value; }
#endif
    }

    public EffectFunctionality SubFunctionality
    {
        get { return _subFunctionality; }

#if UNITY_EDITOR
        set { _subFunctionality = value; }
#endif
    }

    #region Type Specific Properties

    public AttackType AttackType
    {
        get { return _attackType; }

#if UNITY_EDITOR
        set { _attackType = value; }
#endif
    }

    public TargetBehaviourType TargetBehaviour
    {
        get { return _targetBehaviour; }

#if UNITY_EDITOR
        set { _targetBehaviour = value; }
#endif
    }

    public MovementRegions MovementRegions
    {
        get { return _movementRegions; }

#if UNITY_EDITOR
        set { _movementRegions = value; }
#endif
    }

    public KeywordType Keyword
    {
        get { return _keyword; }

#if UNITY_EDITOR
        set { _keyword = value; }
#endif
    }

    public MultipleBreakerType MultipleBreaker
    {
        get { return _multipleBreaker; }

#if UNITY_EDITOR
        set { _multipleBreaker = value; }
#endif
    }

    public TapStateType TapState
    {
        get { return _tapState; }

#if UNITY_EDITOR
        set { _tapState = value; }
#endif
    }

    public DiscardParam DiscardParam
    {
        get { return _discardParam; }

#if UNITY_EDITOR
        set { _discardParam = value; }
#endif
    }

    public LookAtParam LookAtParam
    {
        get { return _lookAtParam; }

#if UNITY_EDITOR
        set { _lookAtParam = value; }
#endif
    }
    
    public int AttackBoost
    {
        get { return _attackBoost; }

#if UNITY_EDITOR
        set { _attackBoost = value; }
#endif
    }
    
    public int CostAdjustmentAmount
    {
        get { return _costAdjustmentAmount; }

#if UNITY_EDITOR
        set { _costAdjustmentAmount = value; }
#endif
    }
    
    public bool ShouldMultiplyVal
    {
        get { return _shouldMultiplyVal; }

#if UNITY_EDITOR
        set { _shouldMultiplyVal = value; }
#endif
    }
    
    public bool AlterFunctionUntilEndOfTurn
    {
        get { return _alterFunctionUntilEndOfTurn; }

#if UNITY_EDITOR
        set { _alterFunctionUntilEndOfTurn = value; }
#endif
    }

    #endregion

    public bool TargetUnspecified()
    {
        switch (_type)
        {
            case EffectFunctionalityType.AttackTarget:
            case EffectFunctionalityType.TargetBehaviour:
            case EffectFunctionalityType.Keyword:
            case EffectFunctionalityType.PowerAttacker:
            case EffectFunctionalityType.MultipleBreaker:
                if (_multipleBreaker == MultipleBreakerType.CrewBreaker)
                    return true;
                return false;
        }

        return true;
    }
    
    public override string ToString()
    {
        string str = GetTypeRepresentation();
        
        if (_target == FunctionTargetType.TargetOther || _shouldMultiplyVal)
            str += $" {_targetingParameter}";
        
        if (_assignedCondition)
            str += $" where{_targetingCondition}";

        if (_connectSubFunctionality)
            str += $" {_connector}";

        if (_subFunctionality)
            str += $"\n\t{_subFunctionality}";

        return str;

        #region Local Functions

        string GetTypeRepresentation()
        {
            switch (_type)
            {
                case EffectFunctionalityType.RegionMovement:
                    return GetRegionMovementString();

                case EffectFunctionalityType.AttackTarget:
                        return _attackType.ToString();

                case EffectFunctionalityType.TargetBehaviour:
                        return _targetBehaviour.ToString();

                case EffectFunctionalityType.Keyword:
                    return _keyword.ToString();

                case EffectFunctionalityType.MultipleBreaker:
                    return _multipleBreaker.ToString();

                case EffectFunctionalityType.ToggleTap:
                    return _tapState.ToString();

                case EffectFunctionalityType.Destroy:
                    string str3 = "Destroy";
                    if (_target == FunctionTargetType.TargetSelf)
                        str3 += " self";
                    return str3;

                case EffectFunctionalityType.Discard:
                    return $"{_target} {_discardParam}";

                case EffectFunctionalityType.LookAtRegion:
                    return $"{_lookAtParam} in {_target}'s {_lookAtParam.lookAtRegion}";

                case EffectFunctionalityType.PowerAttacker:
                    return $"Power Attacker +{_attackBoost}";

                case EffectFunctionalityType.GrantPower:
                    return $"Gets +{_attackBoost}";
                
                case EffectFunctionalityType.GrantFunction:
                    string str2 = "Grant Function";
                    if (_alterFunctionUntilEndOfTurn)
                        str2 += " until the end of turn";
                    return str2;

                case EffectFunctionalityType.CostAdjustment:
                    return $"Adjust cost by {_costAdjustmentAmount}";

                default:
                    return _type.ToString();
            }
        }

        string GetRegionMovementString()
        {
            string str1;

            switch (_target)
            {
                case FunctionTargetType.Player:
                    str1 = "Player moves ";
                    break;
                case FunctionTargetType.Opponent:
                    str1 = "Opponent moves ";
                    break;
                default:
                    str1 = "Move ";
                    break;
            }

            str1 += $"{_movementRegions.moveCount} card ";
            if (_movementRegions.moveCount > 1)
                str1 += "s ";

            if (_movementRegions.fromRegion == RegionType.Deck)
            {
                if (_movementRegions.deckCardMove == DeckCardMoveType.Top)
                    return $"Draw {_movementRegions.countChoice} {_movementRegions.moveCount}";
                
                str1 += $"after searching deck to {_movementRegions.toRegion}";
                if (_movementRegions.showSearchedCard)
                    str1 += " and show it to the opponent";
            }
            else if (_movementRegions.toRegion == RegionType.Deck)
            {
                str1 += $"from {_movementRegions.fromRegion} ";
                if (_movementRegions.deckCardMove == DeckCardMoveType.Top)
                    str1 += $"to top of Deck";
                else
                    str1 += "and shuffle into Deck";
            }
            else
                str1 += $"from {_movementRegions.fromRegion} to {_movementRegions.toRegion}";
            return str1;
        }

        #endregion
    }
}
