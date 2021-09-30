using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    private Vector3 _creatureOriginalPos = Vector3.zero;

    #region Static Data Members

    private static BattleManager _Instance = null;

    public static BattleManager Instance
    {
        get
        {
            if (!_Instance)
                _Instance = FindObjectOfType<BattleManager>();
            return _Instance;
        }
    }

    #endregion
    
    private void Awake()
    {
        int count = FindObjectsOfType<BattleManager>().Length;
        if (count > 1)
            Destroy(gameObject);
        else
            _Instance = this;
    }

    public Coroutine AttemptAttack(bool isPlayer, CardBehaviour target, bool registerPos = true, bool continueAttack = false)
    {
        return StartCoroutine(AttemptAttackRoutine(isPlayer, target, registerPos, continueAttack));
    }
    
    private IEnumerator AttemptAttackRoutine(bool isPlayer, CardBehaviour target, bool registerPos, bool continueAttack)
    {
        Controller controller = GameManager.Instance.GetController(isPlayer);

        if (controller.CurrentlySelected)
        {
            CardObject selectedCardObj = controller.CurrentlySelected;

            if (selectedCardObj.InZone(CardZoneType.BattleZone) && !selectedCardObj.CardInst.IsTapped)
            {
                TargetingLinesHandler.Instance.DisableLines();

                CreatureObject attackingCreatureObj = (CreatureObject) selectedCardObj;
                if (registerPos)
                    _creatureOriginalPos = attackingCreatureObj.transform.position;
                IEnumerator attackRoutine = null;

                if (target is CreatureObject targetCreatureObj)
                {
                    //if (attackedCreatureObj.CardInst.IsTapped)
                        attackRoutine = AttackCreatureRoutine(controller, attackingCreatureObj, targetCreatureObj, continueAttack);
                }
                else if (target is ShieldObject targetShieldObj)
                    attackRoutine = AttackShieldRoutine(controller, attackingCreatureObj, targetShieldObj, isPlayer, continueAttack);

                CreatureObject blockingCreatureObj;
                if (true /*creatureObj can be blocked*/)
                {
                    Coroutine<CreatureObject> blockRoutine = this.StartCoroutine<CreatureObject>(AttemptBlockRoutine(!isPlayer));
                    yield return blockRoutine.coroutine;
                    blockingCreatureObj = blockRoutine.returnVal;
                }

                if (blockingCreatureObj)
                    attackRoutine = AttackCreatureRoutine(controller, attackingCreatureObj, blockingCreatureObj, continueAttack);

                attackingCreatureObj.CardInst.InstanceEffectHandler.TriggerWhenAttacking(true);
                yield return attackRoutine;
                attackingCreatureObj.CardInst.InstanceEffectHandler.TriggerWhenAttacking(false);
            }

            GameDataHandler.Instance.CheckWhileConditions();
        }
    }

    private IEnumerator AttackCreatureRoutine(Controller controller, CreatureObject attackingCreatureObj,
        CreatureObject targetCreatureObj, bool continueAttack)
    {
        float attackTime = GameParamsHolder.Instance.AttackTime;
        Pair<int> attackingCreaturePowerPair = GetAttackingCreaturePower(attackingCreatureObj);
        int attackedCreaturePower = targetCreatureObj.CardInst.Power;

        attackingCreatureObj.transform.DOMove(targetCreatureObj.transform.position, attackTime).SetEase(Ease.InCubic);
        yield return new WaitForSeconds(attackTime);

        if (!continueAttack)
            controller.DeselectCurrentlySelected();
        
        if (attackingCreaturePowerPair.second > attackedCreaturePower)
        {
            if (targetCreatureObj.CardInst.InstanceEffectHandler.IsSlayer)
                yield return DestroyAttackingCreature();
            yield return targetCreatureObj.SendToGraveyard();
        }
        else if (attackingCreaturePowerPair.second == attackedCreaturePower)
        {
            yield return DestroyAttackingCreature();
            yield return targetCreatureObj.SendToGraveyard();
        }
        else
        {
            if (attackingCreatureObj.CardInst.InstanceEffectHandler.IsSlayer)
                yield return targetCreatureObj.SendToGraveyard();
            yield return DestroyAttackingCreature();
        }
        

        if (attackingCreatureObj)
            yield return ResetCreatureRoutine(attackingCreatureObj, attackingCreaturePowerPair, continueAttack);

        #region Local Functions

        IEnumerator DestroyAttackingCreature()
        {
            yield return attackingCreatureObj.SendToGraveyard();
            attackingCreatureObj = null;
        }

        #endregion
    }

    private IEnumerator AttackShieldRoutine(Controller controller, CreatureObject attackingCreatureObj,
        ShieldObject shieldObj, bool isPlayer, bool continueAttack)
    {
        float attackTime = GameParamsHolder.Instance.AttackTime;
        Pair<int> attackingCreaturePowerPair = GetAttackingCreaturePower(attackingCreatureObj);

        attackingCreatureObj.transform.DOMove(shieldObj.transform.position, attackTime).SetEase(Ease.InCubic);
        yield return new WaitForSeconds(attackTime);

        if (!continueAttack)
            controller.DeselectCurrentlySelected();

        PlayerManager opponent = GameManager.Instance.GetManager(!isPlayer);
        yield return opponent.BreakShieldRoutine(shieldObj);

        if (attackingCreatureObj)
            yield return ResetCreatureRoutine(attackingCreatureObj, attackingCreaturePowerPair, continueAttack);
    }

    private IEnumerator AttemptBlockRoutine(bool isPlayer)
    {
        Controller controller = GameManager.Instance.GetController(isPlayer);

        Coroutine<CreatureObject> routine =
            controller.StartCoroutine<CreatureObject>(controller.ProcessBlockingRoutine());
        yield return routine.coroutine;
        CreatureObject blockingCard = routine.returnVal;

        yield return blockingCard;
    }

    private IEnumerator ResetCreatureRoutine(CreatureObject creatureObj, Pair<int> creaturePowerPair,
        bool continueAttack)
    {
        if (creatureObj)
        {
            if (!creaturePowerPair.ValuesAreEqual())
                creatureObj.ResetPower(creaturePowerPair.first);

            if (!continueAttack)
            {
                float attackTime = GameParamsHolder.Instance.AttackTime;

                creatureObj.transform.DOMove(_creatureOriginalPos, attackTime).SetEase(Ease.InCubic);
                yield return new WaitForSeconds(attackTime);

                creatureObj.ToggleTapState();
                _creatureOriginalPos = Vector3.zero;
            }
        }
    }

    private Pair<int> GetAttackingCreaturePower(CreatureObject attackingCreatureObj)
    {
        int originalAttackingCreaturePower = attackingCreatureObj.CardInst.Power;
        int attackingCreaturePower = originalAttackingCreaturePower;

        if (attackingCreatureObj.CardInst.InstanceEffectHandler.IsPowerAttacker)
        {
            attackingCreaturePower += attackingCreatureObj.CardInst.InstanceEffectHandler.PowerAttackBoost;
            attackingCreatureObj.UpdatePower(attackingCreaturePower);
        }

        return new Pair<int>(originalAttackingCreaturePower, attackingCreaturePower);
    }
}
