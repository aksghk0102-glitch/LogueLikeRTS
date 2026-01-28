using UnityEngine;

public class SK_BuffSelf : SkillBase
{
    public override void OnExecute(Entity caster, IDamageable target)
    {
        if (caster == null || data == null)
            return;

        var param = data.ParamsDic;
        float durTime = param.ContainsKey("durTime") ? param["durTime"] : 10f;

        Condition buff = new Condition
        {
            ID = data.ID,
            Tags = CDT_Tag.Buff,
            StackType = CDT_StackType.Additive,
            Duration = durTime,
            StackCount = 1
        };

        buff.Features.Add(new StatFeature(param));

        // 캐스터에 컨디션 할당 후 스탯 체크
        caster.GetCondition(buff);
        caster.MarkDirty();

        ParticleManager.inst.SpawnParticle("buffEff", caster.transform, durTime);
    }
}
