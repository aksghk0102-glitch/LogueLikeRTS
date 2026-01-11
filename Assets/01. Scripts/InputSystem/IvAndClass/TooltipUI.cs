using UnityEngine;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;

public class TooltipData
{
    // 툴팁에 표기될 정보 모음 클래스
    public string name;
    public string targetClass;
    public string desc;
    public Dictionary<string, float> ParamDic;  // 설명의 가변변수 파싱할 데이터
}

public class TooltipUI : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descText;
    public TextMeshProUGUI targetClassText;

    TooltipData curData;
    Tween delayTween;

    private void Awake()
    {
        panel.SetActive(false);
    }

    // 호출 시작
    public void RequestShow(TooltipData data)
    {
        Hide();

        curData = data;

        // 호버 후 0.5초 후에 출력
        delayTween = DOVirtual.DelayedCall(0.5f, () =>
         {
             if (curData != null)
                 Show(curData);
         });
    }

    void Show(TooltipData data)
    {
        panel.SetActive(true);
        nameText.text = data.name;

        string finalDesc = data.desc;
        foreach(var p in data.ParamDic)
        {
            finalDesc = finalDesc
                .Replace("{" + p.Key + "}", p.Value.ToString());
        }
        descText.text = finalDesc;

        //
        panel.transform.DOKill();
        panel.transform.localPosition = Vector3.one * 0.8f;
        panel.transform.DOScale(1f, 0.2f)
            .SetEase(Ease.OutBack);
    }
    public void Hide()
    {
        if (delayTween != null)
        {
            delayTween.Kill();
            delayTween = null;
        }

        curData = null;         // 데이터 파기
        panel.SetActive(false);
    }
}
