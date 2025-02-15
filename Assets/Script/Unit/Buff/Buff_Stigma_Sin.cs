using UnityEngine;

public class Buff_Stigma_Sin : Buff
{
    public override void Init(BattleUnit caster, BattleUnit owner)
    {
        _buffEnum = BuffEnum.Sin;

        _name = "대죄";

        _description = "공격 시 타락을 1 부여합니다.";

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
        receiver.ChangeFall(1);

        return false;
    }
}