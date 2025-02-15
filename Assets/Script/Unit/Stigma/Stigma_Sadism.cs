using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Stigma_Sadism : Stigma
{
    public override void Use(BattleUnit caster, BattleUnit receiver)
    {
        base.Use(caster, receiver);

        Buff_Stigma_Sadism sadism = new();
        caster.SetBuff(sadism, caster);

        if (Tier == StigmaTier.Tier1)
        {
            sadism.SetValue(3);
        }
        else if (Tier == StigmaTier.Tier2)
        {
            sadism.SetValue(5);
        }
       
    }
}