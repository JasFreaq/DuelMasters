using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectFunctionality : ScriptableObject
{
    private EffectFunctionType _type;
    private EffectTargetingParameter _targetingParameter = new EffectTargetingParameter();
    private bool _assignedCondition;
    private EffectTargetingCondition _targetingCondition = new EffectTargetingCondition();
    [HideInInspector] [SerializeField] private EffectFunctionality _subFunctionality;

    #region Type Specific Members

    private AttackType _attackType;
    private TargetBehaviourType _targetBehaviour;
    private MovementRegionsType _movementRegions;
    private KeywordType _keyword;
    private TapStateType _tapState;

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

    #endregion
    
    public EffectFunctionType Type
    {
        get { return _type; }

#if UNITY_EDITOR
        set { _type = value; }
#endif
    }

    public EffectTargetingParameter TargetingParameter
    {
        get { return _targetingParameter; }

#if UNITY_EDITOR
        set { _targetingParameter = value; }
#endif
    }

    public bool AssignedCondition
    {
        get { return _assignedCondition; }

#if UNITY_EDITOR
        set { _assignedCondition = value; }
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
            case EffectFunctionType.AttackTarget:
            case EffectFunctionType.TargetBehaviour:
            case EffectFunctionType.Keyword:
                return false;
        }

        return true;
    }

    public override string ToString()
    {
        return $"{_type} {_targetingParameter} {_targetingCondition}";
    }
}
