using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;
using DG.Tweening;

// 데미지 텍스트 파티클용 컴포넌트

public class DamageTextParticle : MonoBehaviour
{
    TextMeshPro tm;

    float duration = 0.5f;
    float moveY = 1f;

    void Awake()
    {
        tm = GetComponent<TextMeshPro>();
    }

    public void Init(DamageInfo info)
    {
        // 초기화
        transform.DOKill();
        tm.DOKill();
        transform.localScale = Vector3.one;

        // 색상 설정
        Color targetColor = Color.white;
        switch (info.type)
        {
            case DamageType.Physics: targetColor = ColorDefine.PhysicsDmg; break;
            case DamageType.Magic: targetColor = ColorDefine.MagicDmg; break;
            case DamageType.True: targetColor = ColorDefine.TrueDmg; break;
            case DamageType.Heal: targetColor = ColorDefine.Heal; break;
        }

        // 텍스트 구성
        tm.text = Mathf.FloorToInt(info.Damage).ToString();
        tm.color = targetColor;
        tm.alpha = 1f;

        if (info.IsCritical)
            transform.DOPunchScale(Vector3.one * 0.5f, 0.2f, 10, 1f);

        // 4. 닷트윈 연출 (상승 및 페이드아웃)
        transform.DOMoveY(transform.position.y + moveY, duration).SetEase(Ease.OutBack);

        // 좌우 랜덤 퍼짐 (연출 풍성함 추가)
        float randomX = UnityEngine.Random.Range(-0.3f, 0.3f);
        transform.DOMoveX(transform.position.x + randomX, duration);

        tm.DOFade(0, 0.2f)
            .SetDelay(duration - 0.2f)
            .OnComplete(() => gameObject.SetActive(false));
    }

    private void OnDisable()
    {
        transform.DOKill();
        tm.DOKill();
    }
}
