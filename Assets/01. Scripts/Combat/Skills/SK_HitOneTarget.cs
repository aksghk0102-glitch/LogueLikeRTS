using System.Data;
using UnityEngine;

public class SK_HitOneTarget : SkillBase
{
    public override void OnExecute(Entity attacker, IDamageable target)
    {
        UnitStats stats = attacker.GetFinalStats();

        float finalDmg = 0;

        // 계수 계산 *액셀 params에 알맞게 기입
        if (data.ParamsDic.TryGetValue("att", out float aRate))
            finalDmg += stats.attack * aRate;
        if (data.ParamsDic.TryGetValue("mAtt", out float mRate))
            finalDmg += stats.mAttack * mRate;
        if (data.ParamsDic.TryGetValue("def", out float dRate))
            finalDmg += stats.defense * dRate;
        if (data.ParamsDic.TryGetValue("res", out float rRate))
            finalDmg += stats.magicResist * rRate;
        if (data.ParamsDic.TryGetValue("hp", out float hRate))
            finalDmg += stats.maxHP * hRate;


        // 스킬 치명타 발동 가능 여부 조회 후 배율 수정
        
        
        // 데미지 타입 결정 : 0 물리, 1 마법, 2 고정
        DamageType dmgType = DamageType.Physics;
        if(data.ParamsDic.TryGetValue("type", out float typeValue))
            dmgType = (DamageType)(int)typeValue;

        // 데미지 소스 생성
        DamageInfo dmgInfo = new DamageInfo
        {
            Attker = attacker,
            Target = target,
            Damage = finalDmg,
            IsCritical = false,
            Source = DamageSource.Skill,
            type = dmgType
        };

        CombatManager.Inst.EnqueueDamage(dmgInfo);
    }
}
