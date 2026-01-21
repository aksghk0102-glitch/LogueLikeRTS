using UnityEngine;

public static class ColorDefine
{
    public static readonly Color DefautColor = new Color32(255, 255, 255, 255);
    public static readonly Color Clear = new Color32(255,255,255,0);
    public static readonly Color Ghost = new Color32(255,255,255,132);
    
    // 배치 미리보기 오브젝트 시각화용 컬러
    public static readonly Color ValidColor = new Color32(199, 242, 206, 200);
    public static readonly Color InvalidColor = new Color32(242, 199, 199, 200);

    // 진영 별 기본 색상
    public static readonly Color Player = new Color32(77, 199, 243, 255);
    public static readonly Color Enemy = new Color32(214, 60, 87, 255);

    // UI 슬롯 표시용 컬러
    public static readonly Color Empty = new Color32(75, 75, 75, 255);
    public static readonly Color Active = new Color32(227, 220, 67, 255);
    public static readonly Color Passive = new Color32(64, 185, 100, 255);

    // 데미지 파티클
    public static readonly Color PhysicsDmg = new Color32(213, 91, 20, 255);
    public static readonly Color PhysicsCritDmg = new Color32(239, 31, 10, 255);
    public static readonly Color MagicDmg = new Color32(170, 82, 207, 255);
    public static readonly Color MagicCritDmg = new Color32(201, 51, 240, 255);
    public static readonly Color TrueDmg = new Color32(240, 240, 240, 255);
    public static readonly Color Heal = new Color32(149, 232, 103, 255);
}
