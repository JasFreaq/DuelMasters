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
public class MovementZones
{
    public CardZoneType fromZone, toZone;
    public DeckCardMoveType deckCardMove;
    public bool fromBothPlayers, showSearchedCard;

    public CountChoiceType countChoice;
    public int moveCount = 1;
}

[System.Serializable]
public class RaceHolder
{
    public CardParams.Race race;
}

[System.Serializable]
public class DestroyParam
{
    public CardZoneType destroyZone;
    public CountType countType;
    public int lookCount = 1;

    public override string ToString()
    {
        string str = "Destroy ";
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
    public CardZoneType lookAtZone;
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
    [SerializeReference] private CardTargetType _targetCard;
    [SerializeReference] private PlayerTargetType _choosingPlayer;
    [SerializeReference] private PlayerTargetType _targetPlayer;
    [SerializeReference] private bool _assignedCondition, _connectSubFunctionality;
    [SerializeReference] private EffectTargetingParameter _targetingParameter = new EffectTargetingParameter();
    [SerializeReference] private EffectTargetingCondition _targetingCondition = new EffectTargetingCondition();
    [SerializeReference] private ConnectorType _connector;
    [SerializeReference] private EffectFunctionality _subFunctionality;

    #region Type Specific Members

    [SerializeReference] private MovementZones _movementZones = new MovementZones();
    [SerializeReference] private AttackType _attackType;
    [SerializeReference] private TargetBehaviourType _targetBehaviour;
    [SerializeReference] private KeywordType _keyword;
    [SerializeReference] private MultipleBreakerType _multipleBreaker;
    [SerializeReference] private TapStateType _tapState;
    [SerializeReference] private DestroyParam _destroyParam = new DestroyParam();
    [SerializeReference] private DiscardParam _discardParam = new DiscardParam();
    [SerializeReference] private LookAtParam _lookAtParam = new LookAtParam();
    [SerializeReference] private int _powerBoost;
    [SerializeReference] private int _costAdjustmentAmount;

    [SerializeReference] private List<RaceHolder> _vortexRaces = new List<RaceHolder>();
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

    public CardTargetType TargetCard
    {
        get { return _targetCard; }

#if UNITY_EDITOR
        set { _targetCard = value; }
#endif
    }

    public PlayerTargetType ChoosingPlayer
    {
        get { return _choosingPlayer; }

#if UNITY_EDITOR
        set { _choosingPlayer = value; }
#endif
    }

    public PlayerTargetType TargetPlayer
    {
        get { return _targetPlayer; }

#if UNITY_EDITOR
        set { _targetPlayer = value; }
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

    public MovementZones MovementZones
    {
        get { return _movementZones; }

#if UNITY_EDITOR
        set { _movementZones = value; }
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

    public DestroyParam DestroyParam
    {
        get { return _destroyParam; }

#if UNITY_EDITOR
        set { _destroyParam = value; }
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
    
    public int PowerBoost
    {
        get { return _powerBoost; }

#if UNITY_EDITOR
        set { _powerBoost = value; }
#endif
    }
    
    public int CostAdjustmentAmount
    {
        get { return _costAdjustmentAmount; }

#if UNITY_EDITOR
        set { _costAdjustmentAmount = value; }
#endif
    }

    public List<RaceHolder> VortexRaces
    {
        get { return _vortexRaces; }
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
        
        if (_targetCard == CardTargetType.TargetOther || _shouldMultiplyVal)
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
                    if (_targetCard == CardTargetType.TargetSelf)
                        str3 += " self";
                    return str3;

                case EffectFunctionalityType.Discard:
                    return $"{_targetCard} {_discardParam}";

                case EffectFunctionalityType.LookAtRegion:
                    return $"{_lookAtParam} in {_targetCard}'s {_lookAtParam.lookAtZone}";

                case EffectFunctionalityType.PowerAttacker:
                    return $"Power Attacker +{_powerBoost}";

                case EffectFunctionalityType.GrantPower:
                    return $"Gets +{_powerBoost}";
                
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
            string str1 = "";

            switch (_targetPlayer)
            {
                case PlayerTargetType.Player:
                    str1 = "Player moves ";
                    break;
                case PlayerTargetType.Opponent:
                    str1 = "Opponent moves ";
                    break;
            }

            str1 += $"{_movementZones.moveCount} card ";
            if (_movementZones.moveCount > 1)
                str1 += "s ";

            if (_movementZones.fromZone == CardZoneType.Deck)
            {
                if (_movementZones.deckCardMove == DeckCardMoveType.Top)
                    return $"Draw {_movementZones.countChoice} {_movementZones.moveCount}";
                
                str1 += $"after searching deck to {_movementZones.toZone}";
                if (_movementZones.showSearchedCard)
                    str1 += " and show it to the opponent";
            }
            else if (_movementZones.toZone == CardZoneType.Deck)
            {
                str1 += $"from {_movementZones.fromZone} ";
                if (_movementZones.deckCardMove == DeckCardMoveType.Top)
                    str1 += "to top of Deck";
                else
                    str1 += "and shuffle into Deck";
            }
            else
                str1 += $"from {_movementZones.fromZone} to {_movementZones.toZone}";
            return str1;
        }

        #endregion
    }
}
