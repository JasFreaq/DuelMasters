using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldsManager : MonoBehaviour
{
    [SerializeField] private Transform[] _cardHolders = new Transform[5];
    [SerializeField] private string _shieldBreakTriggerName = "BreakShield";
    [SerializeField] private string _shieldUnbreakTriggerName = "UnbreakShield";
    [SerializeField] private string _resetTriggerName = "Reset";

    private int _shieldBreakTriggerHash;
    private int _shieldUnbreakTriggerHash;
    private int _resetTriggerHash;

    private Animator[] _shieldAnimators;
    
    private void Awake()
    {
        _shieldAnimators = GetComponentsInChildren<Animator>();

        _shieldBreakTriggerHash = Animator.StringToHash(_shieldBreakTriggerName);
        _shieldUnbreakTriggerHash = Animator.StringToHash(_shieldUnbreakTriggerName);
        _resetTriggerHash = Animator.StringToHash(_resetTriggerName);
    }
}
