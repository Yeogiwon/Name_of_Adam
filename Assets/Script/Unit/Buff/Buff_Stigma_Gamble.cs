using UnityEngine;

public class Buff_Stigma_Gamble : Buff
{    public override void Init(BattleUnit caster, BattleUnit owner)
    {
        _buffEnum = BuffEnum.Gamble;

        _name = "����";

        _description = "����.";

        _count = -1;

        _countDownTiming = ActiveTiming.NONE;

        _buffActiveTiming = ActiveTiming.BEFORE_ATTACK;

        _caster = caster;

        _owner = owner;

        _statBuff = false;

        _dispellable = false;

        _stigmaBuff = true;
    }

    public override bool Active(BattleUnit caster, BattleUnit receiver)
    {
        if (6 >= Random.Range(0, 10))
        {
            caster.ChangedDamage += caster.BattleUnitTotalStat.ATK;
        }
        else
        {
            caster.ChangedDamage -= caster.BattleUnitTotalStat.ATK / 2;
        }

        return false;
    }
}