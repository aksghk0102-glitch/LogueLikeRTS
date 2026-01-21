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
    float moveY = 0.5f;
    float critScale = 1.4f;

    void Awake()
    {
        tm = GetComponentInChildren<TextMeshPro>();
    }

    public void Init(DamageInfo info)
    {
        // 초기화
        Refresh();

        // 색상 설정
        Color targetColor = GetTextColor(info.type, info.IsCritical);


        // 텍스트 구성
        tm.text = Mathf.FloorToInt(info.Damage).ToString();
        tm.color = targetColor;
        tm.alpha = 1f;

        if (info.IsCritical)
            transform.localScale = Vector3.one * critScale;

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
        Refresh();
    }

    void Refresh()
    {
        transform.DOKill(true);
        tm.DOKill(true);
        transform.localScale = Vector3.one;
    }

    Color GetTextColor(DamageType type, bool isCrit)
    {
        if (isCrit)
        {
            switch (type)
            {
                case DamageType.Physics: return ColorDefine.PhysicsCritDmg;
                case DamageType.Magic: return ColorDefine.MagicCritDmg;
                case DamageType.True: return ColorDefine.TrueDmg;
                case DamageType.Heal: return ColorDefine.Heal;
                default: return Color.white;
            }
        }

        switch (type)
        {
            case DamageType.Physics: return ColorDefine.PhysicsDmg;
            case DamageType.Magic: return ColorDefine.MagicDmg;
            case DamageType.True: return ColorDefine.TrueDmg;
            case DamageType.Heal: return ColorDefine.Heal;
            default: return Color.white;
        }
    }
}
