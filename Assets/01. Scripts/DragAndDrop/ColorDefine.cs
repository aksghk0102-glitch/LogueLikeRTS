using UnityEngine;

public static class ColorDefine
{
    const int alpha = 200;

    public static readonly Color DefautColor = new Color32(255, 255, 255, 255);

    // 배치 미리보기 오브젝트 시각화용 컬러
    public static readonly Color ValidColor = new Color32(199, 242, 206, alpha);
    public static readonly Color InvalidColor = new Color32(242, 199, 199, alpha);
}
