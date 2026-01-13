using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// 하단부 5종의 클래스 슬롯에 들어갈 UI 컴포넌트
// 핵심 구현 기능
// 1. 스킬 착용 상태를 점 4개로 유저에게 피드백
// 2. 인벤토리를 끈 상태에서 드래그 하면 필드에 건물 설치

public class ClassSlotUI : MonoBehaviour,
        IPointerClickHandler, IBeginDragHandler,
    IDragHandler, IEndDragHandler
{
    [Header("Class")]
    public UnitClassType classType;

    [Header("UI Reference")]
    public Image[] stateDots;       // 스킬 상태 표시용
    public TextMeshProUGUI levelText;   // 강화 레벨 표시 텍스트

    CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void RefreshSlotUI(UnitSkillSet data)
    {
        if(data.Class != classType)
        {
            return;
        }

        //levelText.text = data.Level.ToString();

        stateDots[0].color = string.IsNullOrEmpty(data.ActiveID) ?
            ColorDefine.Empty : ColorDefine.Active;

        for (int i = 0; i < 3; i++)
        {
            stateDots[i+1].color = string.IsNullOrEmpty(data.PassiveID[i]) ?
                ColorDefine.Empty : ColorDefine.Passive;
        }
    }


    #region 마우스 조작 기능 모음
    bool isDragging = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isDragging) return;

        // 단순 클릭 시 정보 판넬 출력
        UIStateManager.inst.ClickClassSlot(classType);  // 클릭만으로 껐다 켰다 할 수 있게 하기 위해 로직 분리
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 건설을 위한 조건 체크
        // 1) 인벤토리가 열려 있지 않을 때
        // 2) 현재 게임 상태가 준비 단계일 때
        bool closeIventory = !UIStateManager.inst.IsInvenOpen;
        bool isReadyPhase = GameManager.inst.curPhase == GamePhase.Ready;

        if(closeIventory && isReadyPhase)
        {
            // 캔버스 그룹의 투명도를 낮춰서 피드백
            if (canvasGroup != null)
                canvasGroup.alpha = 0.7f;

            BuildManager.inst.StartBuild(classType);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        BuildManager.inst.OnDrag(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (canvasGroup != null)
            canvasGroup.alpha = 1.0f;

        // 드래그 종료 요청
        BuildManager.inst.RequestBuild();
    }
    #endregion
}
