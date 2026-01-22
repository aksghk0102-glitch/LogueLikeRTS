using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;

// 아이템 툴팁 표시
// 툴팁 관련 클래스 이름이 겹치는 게 많은 상태인데
// 범용 툴팁은 TooltipsUI로 사용 중이니 참고...

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
    public TextMeshProUGUI targetClassText;
    public TextMeshProUGUI descText;

    RectTransform rect;
    RectTransform parentRect;

    TooltipData curData;
    Tween delayTween;

    [Header("Offset Settings")]
    public Vector2 offset = new Vector2(10f, -10f);

    private void Awake()
    {
        panel.SetActive(false);
        rect = panel.GetComponent<RectTransform>();
        parentRect = transform.parent.GetComponent<RectTransform>();

        // 피벗 고정 = 좌상단
        rect.pivot = new Vector2(0, 1);
    }

    void UpdatePanelPos()
    {
        // 1. 스크린 좌표(마우스)를 부모의 로컬 좌표로 변환
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            Input.mousePosition,
            null, // Overlay 모드이므로 Camera는 null
            out localPos
        );

        // 2. 오프셋 적용
        Vector2 targetPos = localPos + offset;

        // 3. 화면 밖 보정 (Screen 좌표 기준으로 계산 후 로컬에 적용)
        float width = rect.rect.width;
        float height = rect.rect.height;

        // 현재 마우스의 스크린 좌표 기준 체크
        Vector2 mousePos = Input.mousePosition;

        // 오른쪽 이탈 방지
        if (mousePos.x + offset.x + width > Screen.width)
        {
            targetPos.x -= (width + offset.x * 2);
        }

        // 아래쪽 이탈 방지
        if (mousePos.y + offset.y - height < 0)
        {
            targetPos.y += (height - offset.y * 2);
        }

        // 4. 최종 로컬 포지션 대입
        rect.localPosition = targetPos;
    }

    // 호출 시작
    public void RequestShow(TooltipData data)
    {
        Hide();

        curData = data;

        // 호버 후 0.5초 후에 출력
        delayTween = DOVirtual.DelayedCall(0.3f, () =>
         {
             if (curData != null)
                 Show(curData);
         });
    }

    void Show(TooltipData data)
    {
        Debug.Log($"현재 들어온 파라미터 개수: {data.ParamDic?.Count}");

        panel.SetActive(true);
        nameText.text = data.name;
        targetClassText.text = "착용 가능 : " + GetTargetClassStr(data.targetClass);

        string finalDesc = data.desc;
        if(data.ParamDic != null)
            foreach(var p in data.ParamDic)
            {
                string targetKey = "{" +p.Key.Trim() +"}";
                finalDesc = finalDesc.Replace(targetKey, p.Value.ToString());
            }
        descText.text = "효과 : " + finalDesc;

        // UI 크기 재계산으로 오류 방지
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);

        // 위치 업데이트
        UpdatePanelPos();

        // 연출
        panel.transform.DOKill();
        panel.transform.localScale = Vector3.one * 0.8f;
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

    string GetTargetClassStr(string raw)
    {
        if (string.IsNullOrEmpty(raw))
            return "";

        // '/'의 갯수 확인
        int count = 0;
        foreach (char c in raw)
            if (c == '/') count++;

        if (count == 4)
            return "모든 클래스";

        string[] array = raw.Split('/');
        for (int i = 0; i < array.Length; i++)
            array[i] = ConvertKR(array[i].Trim());

        return string.Join(", ", array);
    }

    string ConvertKR(string name)
    {
        return name switch
        {
            "Babarian" => "바바리안",
            "Knight" => "나이트",
            "Rogue" => "로그",
            "Ranger" => "레인저",
            "Mage" => "메이지",
            _ => name,
        };
    }
}
