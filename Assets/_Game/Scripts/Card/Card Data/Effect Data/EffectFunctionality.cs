using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectFunctionality : ScriptableObject
{
    [SerializeReference] private EffectFunctionalityType _type;
    [SerializeReference] private FunctionTargetType _target;
    [SerializeReference] private bool _assignedCondition;
    [SerializeReference] private EffectTargetingParameter _targetingParameter = new EffectTargetingParameter();
    [SerializeReference] private EffectTargetingCondition _targetingCondition = new EffectTargetingCondition();
    [SerializeReference] private EffectFunctionality _subFunctionality;

    #region Type Specific Members

    [HideInInspector] [SerializeField]
    private AttackType _attackType;
    [HideInInspector] [SerializeField]
    private TargetBehaviourType _targetBehaviour;
    [HideInInspector] [SerializeField]
    private MovementRegionsType _movementRegions;
    [HideInInspector] [SerializeField]
    private KeywordType _keyword;
    [HideInInspector] [SerializeField]
    private MultipleBreakerType _multipleBreaker;
    [HideInInspector] [SerializeField]
    private TapStateType _tapState;
    [HideInInspector] [SerializeField]
    private int _powerAttackerBoost;
    [HideInInspector] [SerializeField]
    private int _attackBoostGrant;

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

    public MovementRegionsType MovementRegions
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

    public int PowerAttackerBoost
    {
        get { return _powerAttackerBoost; }

#if UNITY_EDITOR
        set { _powerAttackerBoost = value; }
#endif
    }

    public int AttackBoostGrant
    {
        get { return _attackBoostGrant; }

#if UNITY_EDITOR
        set { _attackBoostGrant = value; }
#endif
    }

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

    public EffectFunctionality SubFunctionality
    {
        get { return _subFunctionality; }

#if UNITY_EDITOR
        set { _subFunctionality = value; }
#endif
    }

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
        
        if (_target == FunctionTargetType.TargetOther)
        {
            str += $" {_targetingParameter}";
            if (_assignedCondition)
                str += $" where{_targetingCondition}";
        }
        else if (_assignedCondition)
            str += $" {_targetingCondition}";

        if (_subFunctionality)
            str += $"\n\t{_subFunctionality}";
        return str;

        #region Local Functions

        string GetTypeRepresentation()
        {
            switch (_type)
            {
                case EffectFunctionalityType.AttackTarget:
                        return _attackType.ToString();

                case EffectFunctionalityType.TargetBehaviour:
                        return _targetBehaviour.ToString();

                case EffectFunctionalityType.RegionMovement:
                    if (_movementRegions == MovementRegionsType.Draw)
                    {
                        return $"{_movementRegions}";
                    }
                    return $"Move target from {_movementRegions}";

                case EffectFunctionalityType.Keyword:
                    return _keyword.ToString();

                case EffectFunctionalityType.MultipleBreaker:
                    return _multipleBreaker.ToString();

                case EffectFunctionalityType.ToggleTap:
                    return _tapState.ToString();

                case EffectFunctionalityType.PowerAttacker:
                    return $"Power Attacker +{_powerAttackerBoost}";

                case EffectFunctionalityType.GrantPower:
                    return $"Gets +{_attackBoostGrant}";

                case EffectFunctionalityType.Destroy:
                    string str = "Destroy";
                    if (_target == FunctionTargetType.TargetSelf)
                        str += " self";
                    return str;

                    //case EffectFunctionType.AttacksEachTurnIfAble:
                    //    break;
                    //case EffectFunctionType.CantBeBlocked:
                    //    break;
                    //case EffectFunctionType.Draw:
                    //    break;
                    //case EffectFunctionType.SearchAndShuffle:
                    //    break;
                    //case EffectFunctionType.LookAtShield:
                    //    break;
                    //case EffectFunctionType.CostReduction:
                    //    break;
            }

            return _type.ToString();
        }

        #endregion
    }
}
