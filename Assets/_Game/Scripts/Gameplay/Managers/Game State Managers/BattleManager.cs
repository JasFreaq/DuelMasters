using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
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

    public Coroutine AttemptAttack(bool isPlayer, CardBehaviour target, bool continueAttack = false)
    {
        return StartCoroutine(AttemptAttackRoutine(isPlayer, target, continueAttack));
    }
    
    private IEnumerator AttemptAttackRoutine(bool isPlayer, CardBehaviour target, bool continueAttack)
    {
        Controller controller = GameManager.Instance.GetController(isPlayer);

        if (controller.CurrentlySelected)
        {
            CardObject selectedCardObj = controller.CurrentlySelected;

            if (selectedCardObj.InZone(CardZoneType.BattleZone) && !selectedCardObj.CardInst.IsTapped)
            {
                TargetingLinesHandler.Instance.DisableLines();

                CreatureObject creatureObj = (CreatureObject)selectedCardObj;
                IEnumerator attackRoutine = null;

                if (target is CreatureObject attackedCreatureObj)
                {
                    if (attackedCreatureObj.CardInst.IsTapped)
                        attackRoutine = AttackCreatureRoutine(controller, creatureObj, attackedCreatureObj, continueAttack);
                }
                else if (target is ShieldObject shieldObj)
                    attackRoutine = AttackShieldRoutine(controller, creatureObj, shieldObj, isPlayer, continueAttack);

                CreatureObject blockingCreatureObj;
                if (true /*creatureObj can be blocked*/)
                {
                    Coroutine<CreatureObject> blockRoutine = this.StartCoroutine<CreatureObject>(AttemptBlockRoutine(!isPlayer));
                    yield return blockRoutine.coroutine;
                    blockingCreatureObj = blockRoutine.returnVal;
                }

                if (blockingCreatureObj)
                    attackRoutine = AttackCreatureRoutine(controller, creatureObj, blockingCreatureObj, continueAttack);

                yield return attackRoutine;
            }
        }
    }

    private IEnumerator AttackCreatureRoutine(Controller controller, CreatureObject creatureObj,
        CreatureObject attackedCreatureObj, bool continueAttack)
    {
        Vector3 creaturePos = creatureObj.transform.position;
        float attackTime = GameParamsHolder.Instance.AttackTime;

        int attackingCreaturePower = creatureObj.CardData.Power;
        if (creatureObj.CardInst.IsPowerAttacker)
        {
            attackingCreaturePower += creatureObj.CardInst.PowerBoost;
            creatureObj.DisplayPowerAttack(attackingCreaturePower);
        }
        int attackedCreaturePower = attackedCreatureObj.CardData.Power;

        creatureObj.transform.DOMove(attackedCreatureObj.transform.position, attackTime).SetEase(Ease.InCubic);
        yield return new WaitForSeconds(attackTime);

        if (!continueAttack)
            controller.DeselectCurrentlySelected();

        if (creatureObj.CardInst.IsPowerAttacker)
            creatureObj.ResetPowerAttack();

        if (attackingCreaturePower > attackedCreaturePower)
            yield return attackedCreatureObj.SendToGraveyard();
        else if (attackingCreaturePower == attackedCreaturePower)
        {
            yield return creatureObj.SendToGraveyard();
            creatureObj = null;
            yield return attackedCreatureObj.SendToGraveyard();
        }
        else
        {
            yield return creatureObj.SendToGraveyard();
            creatureObj = null;
        }

        if (creatureObj && !continueAttack)
            yield return ResetCreaturePosRoutine(creaturePos, creatureObj);
    }

    private IEnumerator AttackShieldRoutine(Controller controller, CreatureObject creatureObj,
        ShieldObject shieldObj, bool isPlayer, bool continueAttack)
    {
        float attackTime = GameParamsHolder.Instance.AttackTime;
        Vector3 creaturePos = creatureObj.transform.position;

        creatureObj.transform.DOMove(shieldObj.transform.position, attackTime).SetEase(Ease.InCubic);
        yield return new WaitForSeconds(attackTime);

        if (!continueAttack)
            controller.DeselectCurrentlySelected();

        PlayerManager opponent = GameManager.Instance.GetManager(!isPlayer);
        StartCoroutine(opponent.BreakShieldRoutine(shieldObj));

        if (!continueAttack)
            yield return ResetCreaturePosRoutine(creaturePos, creatureObj);
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

    private IEnumerator ResetCreaturePosRoutine(Vector3 creaturePos, CreatureObject creatureObj)
    {
        float attackTime = GameParamsHolder.Instance.AttackTime;

        creatureObj.transform.DOMove(creaturePos, attackTime).SetEase(Ease.InCubic);
        yield return new WaitForSeconds(attackTime);
        creatureObj.ToggleTapState();
    }
}
