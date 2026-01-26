using UnityEngine;

// 모든 스킬을 관리하기 위한 추상 클래스입니다.
// 각각의 스킬 로직 클래스는 이 클래스를 상속받습니다.

public abstract class SkillBase
{
    protected SkillData data;

    public virtual void Init(SkillData data)
    {
        this.data = data;
    }

    // 실제 실행 로직 => 애니메이션 이벤트에서 호출
    public virtual void Execute(Entity attacker, IDamageable target)
    {
        if (!string.IsNullOrEmpty(data.EffectID) || data.EffectID != "None") {
            ParticleManager.inst.SpawnParticle(data.EffectID, attacker.transform.position);
        }

        // 사운드 출력 위치
        //SoundManager.inst.PlaySFX(data.SoundID);

        OnExecute(attacker, target);
    }

    public abstract void OnExecute(Entity attacker, IDamageable target);
}
