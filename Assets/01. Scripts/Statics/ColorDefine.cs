using UnityEngine;

public static class ColorDefine
{
    public static readonly Color DefautColor = new Color32(255, 255, 255, 255);

    // 배치 미리보기 오브젝트 시각화용 컬러
    public static readonly Color ValidColor = new Color32(199, 242, 206, 200);
    public static readonly Color InvalidColor = new Color32(242, 199, 199, 200);

    // 진영 별 기본 색상
    public static readonly Color Player = new Color32(77, 199, 243, 255);
    public static readonly Color Enemy = new Color32(214, 60, 87, 255);
}
