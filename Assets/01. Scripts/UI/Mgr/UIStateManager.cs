using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using TMPro;

// 모든 UI를 총괄하는 중계자.
// 각종 판넬, 팝업, 툴팁 등 이 매니저를 통해 On/Off 관리
// 
// 1/22 꽤 지저분해지고 있는데 언제 구조 통일 좀 시켜야 할듯

public class UIStateManager : MonoBehaviour
{
    public static UIStateManager inst;

    [Header("UI Reference")]
    public CanvasGroup dim_CanvasGroup;     // 딤 연출 패널 캔버스 그룹(인벤토리 활성화 시 주변 시야 어둡게)
    public CanvasGroup iv_CanvasGroup;      // 인벤토리 캔버스 그룹
    public RectTransform iv_Rect;           // 인벤토리 위치 -> 연출 시 활용
    public RectTransform bagIconRect;       // 가방 아이콘 위치 -> 연출 시 활용

    [SerializeField] float duration = 0.3f;    // 인벤토리를 열고 닫을 때 연출 속도
    bool isTween = false;                       // 연출 중복 실행 방지

    [Header("State")]
    bool isInvenOpen = false;
    bool isPopUpOpen = false;
    public bool IsInvenOpen => isInvenOpen;
    public bool IsPopUpOpen => isPopUpOpen;

    Vector2 center = Vector2.zero;

    [Header("Panels")]
    [SerializeField] ClassInfoUI classInfoUI;     // 클래스 별 정보 판넬
    // 추가할 사항
    // 퍼즈 시 메뉴
    [SerializeField] CombatStatUI combStatUI;       // 전투 통계 판넬

    [Header("ToolTips")]
    public TooltipUI tooltipUI;         // 마우스 호버로 따라다니는 스킬 툴팁 
    public GeneralTooltips tooltips;

    [Header("PopUp")]
    [SerializeField] GameObject confirmPopUp;
    [SerializeField] Button confirmBtn;
    [SerializeField] Button cancleBtn;
    [SerializeField] TextMeshProUGUI popUpText;

    [Header("Start Btn")]
    public Button startBtn;
    public Button combatStatOpenBtn;

    private void Awake()
    {
        if (inst == null)
            inst = this;
        else
            Destroy(gameObject);

        if (startBtn != null)
            startBtn.onClick.AddListener(OnClickStartBtn);

        if (combatStatOpenBtn != null)
            combatStatOpenBtn.onClick.AddListener(CombatStatOpen);
    }

    void Start()
    {
        // 각 UI 초기화
        iv_CanvasGroup.alpha = 0;
        iv_CanvasGroup.blocksRaycasts = false;

        dim_CanvasGroup.alpha = 0;
        dim_CanvasGroup.blocksRaycasts = false;

        iv_Rect.anchoredPosition = bagIconRect.anchoredPosition;
        iv_Rect.localScale = Vector3.zero;

        if (combStatUI != null)
            combStatUI.Close();
        if (tooltips != null)
            tooltips.Hide();
    }

    public void ShowClassInfo(UnitClassType type)
    {
        if (classInfoUI == null) return;

        // 사운드 호출
        SoundManager.inst.PlaySFX("Interface 3-1");

        classInfoUI.gameObject.SetActive(true);
        classInfoUI.Open(type);
    }
    public void ClickClassSlot(UnitClassType type)
    {
        if (classInfoUI == null) return;

        // 사운드 호출
        SoundManager.inst.PlaySFX("Interface 3-1");

        if (classInfoUI.curType == type)
            classInfoUI.Exit();
        else if(classInfoUI.curType != type)
            ShowClassInfo(type);
    }

    public void ShowPopUp(string msg, Action onConfirm, Action onCancle = null)
    {
        if (IsPopUpOpen)
            return;

        isPopUpOpen = true;

        popUpText.text = msg;

        // 버튼 초기화 및 리스너 연결
        confirmBtn.onClick.RemoveAllListeners();
        cancleBtn.onClick.RemoveAllListeners();

        confirmBtn.onClick.AddListener(() =>
        {
            // 사운드 호출
            SoundManager.inst.PlaySFX("Interface 3-1");
         
            onConfirm?.Invoke();

            HidePopUp();
        });

        if(onCancle != null)
        {
            cancleBtn.onClick.AddListener(() =>
            {
                onCancle?.Invoke();
                HidePopUp();
            });
        }
        else
        {
            cancleBtn.onClick.AddListener(() =>
            {
                HidePopUp();
            });
        }

        // 딤 패널 활성화
        dim_CanvasGroup.blocksRaycasts = true;
        dim_CanvasGroup.DOFade(0.8f, duration);

        // 팝업 연출
        confirmPopUp.SetActive(true);
        confirmPopUp.transform.localPosition = Vector3.zero;
        confirmPopUp.transform.DOScale(Vector3.one, duration)
            .SetEase(Ease.OutBack);

    }

    public void CanclePopUp()
    {
        // 사운드 호출
        SoundManager.inst.PlaySFX("Interface 6-5");

        HidePopUp();
    }
    
    public void HidePopUp()
    {
        if (!IsPopUpOpen)
            return;

        confirmPopUp.transform.DOScale(Vector3.zero, duration)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                confirmPopUp.SetActive(false);
                isPopUpOpen = false;

                if (!IsInvenOpen)
                {
                    dim_CanvasGroup.blocksRaycasts = false;
                    dim_CanvasGroup.DOFade(0f, duration);
                }
            });
    }

    public void CombatStatOpen()
    {
        if (combStatUI == null)
            return;

        // 사운드 호출
        SoundManager.inst.PlaySFX("Interface 3-1");
        
        combStatUI.Open();
        CloseIvWithOutSound();
    }


    // Update is called once per frame
    void Update()
    {
        if (isTween || IsPopUpOpen)
            return;

        if( (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Tab) )
            && !isTween)
        {
            ToggleInventory();
        }
    }


    public void OnClickBagBtn()
    {
        if (!isTween)
            ToggleInventory();
    }

    public void ToggleInventory()
    {
        if (IsPopUpOpen)
            return;

        isInvenOpen = !isInvenOpen;
        isTween = true;

        // 가방 아이콘을 살짝 흔들기
        bagIconRect.DOKill();
        bagIconRect.localScale = Vector3.one;
        bagIconRect.DOPunchScale(new Vector3(0.2f, 0.2f, 0f), 0.3f, 10, 1);

        if (isInvenOpen)
            OpenInventory();
        else
            CloseInventory();
    }

    public void ShowTooltip(string title, string text)
    {
        //tooltipUI.gameObject.SetActive(true);
        tooltips.SetText(title, text);
    }
    public void HideTooltip()
    {
        tooltips.Hide();
    }

    void OpenInventory()
    {
        // 사운드 호출
        SoundManager.inst.PlaySFX("Bag Handle 1-5");

        iv_CanvasGroup.blocksRaycasts = true;
        iv_CanvasGroup.interactable = true;

        // 딤 효과 연출
        dim_CanvasGroup.blocksRaycasts = true;
        dim_CanvasGroup.DOFade(1f, duration);

        // 가방 아이콘 쪽으로 위치 조정
        iv_Rect.anchoredPosition = bagIconRect.anchoredPosition;

        // 이동 및 연출
        iv_Rect
            .DOAnchorPos(center, duration)
            .SetEase(Ease.OutBack);
        iv_Rect
            .DOScale(Vector3.one, duration)
            .SetEase(Ease.OutBack);
        iv_CanvasGroup
            .DOFade(1f, duration)
            .OnComplete(()=> isTween = false);
    }
    void CloseInventory()
    {
        // 사운드 호출
        SoundManager.inst.PlaySFX("Interface 6-5");

        CloseIvWithOutSound();
    }
    void CloseIvWithOutSound()
    {
        iv_CanvasGroup.blocksRaycasts = false;
        iv_CanvasGroup.interactable = false;

        // 딤 효과 종료 연출
        dim_CanvasGroup.blocksRaycasts = false;
        dim_CanvasGroup.DOFade(0f, duration);

        // 이동 > 이거 안되는 거 같은데
        iv_Rect
            .DOAnchorPos(bagIconRect.anchoredPosition, duration)
            .SetEase(Ease.InBack);
        iv_Rect.DOScale(Vector3.zero, duration)
            .SetEase(Ease.InBack);
        iv_CanvasGroup.DOFade(0f, duration)
            .OnComplete(() =>
            {
                isTween = false;
            });
    }


    string remainCostMsg = "아직 코스트가\n남아 있습니다.\n정말 전투를\n시작하겠습니까?";
    void OnClickStartBtn()
    {
        if(GameManager.inst.CurCost > 0)
        {
            ShowPopUp(remainCostMsg,
            () => {
                if (GameManager.inst.curPhase == GamePhase.Ready)
                    GameManager.inst.SetPhase(GamePhase.Battle);
            });
        }
        else
        {
            if (GameManager.inst.curPhase == GamePhase.Ready)
                GameManager.inst.SetPhase(GamePhase.Battle);
        }
    }
}
