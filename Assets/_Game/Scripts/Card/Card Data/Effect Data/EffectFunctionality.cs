using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectFunctionality : ScriptableObject
{
    private EffectFunctionalityType _type;
    private FunctionTargetType _target;
    private EffectFunction _effectFunction;
    private bool _assignedCondition;
    private EffectTargetingParameter _targetingParameter = new EffectTargetingParameter();
    private EffectTargetingCondition _targetingCondition = new EffectTargetingCondition();
    [HideInInspector] [SerializeField] private EffectFunctionality _subFunctionality;

    #region Type Specific Members

    private AttackType _attackType;
    private TargetBehaviourType _targetBehaviour;
    private MovementRegionsType _movementRegions;
    private KeywordType _keyword;
    private TapStateType _tapState;
    private int _powerAttackerBoost;
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

    public bool CheckParameter()
    {
        switch (_type)
        {
            case EffectFunctionalityType.AttackTarget:
            case EffectFunctionalityType.TargetBehaviour:
            case EffectFunctionalityType.Keyword:
                return false;
        }

        return true;
    }
    
    public override string ToString()
    {
        string str = GetTypeRepresentation();
        if (_target == FunctionTargetType.TargetOther) 
            str += $" {_targetingParameter}";
        if (_assignedCondition)
            str += $" {_targetingCondition}";
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

                case EffectFunctionalityType.ToggleTap:
                    return _tapState.ToString();

                case EffectFunctionalityType.PowerAttacker:
                    return _powerAttackerBoost.ToString();

                case EffectFunctionalityType.GrantPower:
                    return _attackBoostGrant.ToString();

                case EffectFunctionalityType.MultipleBreaker:
                    break;
                    //case EffectFunctionType.Destroy:
                    //    break;
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
