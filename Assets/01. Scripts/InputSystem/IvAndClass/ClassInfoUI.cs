using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 클래스 인포 판넬에 붙여서 쓸 컴포넌트

public class ClassInfoUI : MonoBehaviour
{
    [Header("Current Target Class")]
    public UnitClassType curType = UnitClassType.Init;       // 현재 정보 출력 중인 클래스
    Barracks targetBarrack;             // 현재 출력 중인 배럭의 인스턴스 참조

    [Header("Reference")]
    public TextMeshProUGUI classNameText;
    public TextMeshProUGUI hpText;          // 체력 표시 텍스트 123(현재)/200(최대) 형태
    public Image hpGauge;                 // 체력 바
    
    public Button upgradeBtn;               // 업그레이드 버튼
    public TextMeshProUGUI levelText;                  // 레벨 표시 텍스트

    public Button buildBtn;                 // 건설/판매 버튼
    public TextMeshProUGUI buildText;       // 텍스트 수정용

    public Button unitInfoBtn;              // 유닛 정보 팝업 표시

    public SkillSlotVisual activeSlotImg;             // 액티브 스킬 슬롯
    public SkillSlotVisual[] passiveSlotImg;          // 패시브 스킬 슬롯
    public Button exitBtn;

    public TooltipsUI tooltips;

    string msg_CantSell = "이 건물은 팔 수 없습니다.";
    //string msg_CantUpgarade = "이 건물은 강화할 수 없습니다.";

    private void Awake()
    {
        gameObject.SetActive(false); // 기본적으로는 꺼둠
        if (exitBtn != null)
            exitBtn.onClick.AddListener(Exit);
    }

    public void Open(UnitClassType type, UnitFaction faction = UnitFaction.Player)
    {
        curType = type;

        // 이전 이벤트 구독 해제
        Unsubscribe();

        // 배럭 상태 조회 후 등록
        targetBarrack = ObjectManager.Inst.GetBarracks(type);

        buildBtn.onClick.RemoveAllListeners();
        upgradeBtn.onClick.RemoveAllListeners();

        if(targetBarrack != null)
        {
            // 설치가 확인된 경우
            
            // 이벤트 구독
            targetBarrack.OnHpChanged += UpdateHpUI;
            targetBarrack.OnDestroy += OnTargetDestroyed;

            UpdateHpUI(targetBarrack.curHp, targetBarrack.maxHp);

            buildText.text = "판매";
            buildBtn.onClick.AddListener(OnClickSell);

            upgradeBtn.interactable = true;
            upgradeBtn.onClick.AddListener(OnClickUpgrade);
            levelText.text = $"Lv.{targetBarrack.CurLevel}";
        }
        else
        {
            // 설치가 안된 경우

            UpdateHpUI(); // 초기화 어떻게 할지 고민...

            buildText.text = "건설";
            buildBtn.onClick.AddListener(OnClickBuild);

            upgradeBtn.interactable = false;
            levelText.text = "Lv.0";
        }

        classNameText.text = ConvertToNameStr(type);

        gameObject.SetActive(true);
        Refresh();
    }

    void UpdateHpUI(float cur = 100f, float max = 100f)
    {
        hpGauge.fillAmount = cur / max;
        hpText.text = $"{Mathf.CeilToInt(cur)} / {max}";
    }

    void OnTargetDestroyed()
    {
        // 건물이 파괴된 경우 바로 호출
        Unsubscribe();
        targetBarrack = null;
        
        UpdateHpUI();
        buildText.text = "건설";
        buildBtn.onClick.RemoveAllListeners();
        buildBtn.onClick.AddListener(OnClickBuild);

        upgradeBtn.interactable = false;
        levelText.text = "Lv.0";
    }

    void Unsubscribe()
    {
        if(targetBarrack != null)
        {
            targetBarrack.OnHpChanged -= UpdateHpUI;
            targetBarrack.OnDestroy -= OnTargetDestroyed;
        }
    }


    public void Refresh()
    {
        if (targetBarrack != null && targetBarrack.Faction != UnitFaction.Player)
            return;

        // 장비 상태 관리자에서 정보 받아오기
        UnitSkillSet data = EquipManager.inst.GetUnitSkillSet(curType);

        //Debug.Log($"[ClassInfoUI] Refreshing {curType}. Active: {data.ActiveID}, Passives: {string.Join(", ", data.PassiveID)}");

        // 액티브 스킬 표시 및 툴팁 갱신
        SkillData active = InventoryManager.inst.GetSkillData(data.ActiveID);
        activeSlotImg.SetVisual(active);
        if (activeSlotImg.TryGetComponent(out TooltipTrigger aTrigger))
            aTrigger.SetData(active);

        for (int i = 0; i < passiveSlotImg.Length; i++)
        {
            SkillData passive = InventoryManager.inst.GetSkillData(data.PassiveID[i]);
            passiveSlotImg[i].SetVisual(passive);

            if (passiveSlotImg[i].TryGetComponent(out TooltipTrigger pTrigger))
                pTrigger.SetData(passive);
        }

        TooltipRefesh();
    }

    void TooltipRefesh()
    {
        if (tooltips == null) return;

        tooltips.title = ConvertToNameStr(curType);
        tooltips.content = GetClassTooltip(curType);
    }

    // OnClickActiveSlot과 OnClickPassiveSlot은
    // 하이어라키 상에서 버튼 이벤트로 연결되어 있음
    public void OnClickActiveSlot()
    {
        EquipManager.inst.UnequipActive(curType);
        Refresh();
    }

    public void OnClickPassiveSlot(int idx)
    {
        EquipManager.inst.UnequipPassive(curType, idx);
        Refresh();
    }


    // 건설 버튼에 연결
    void OnClickBuild()
    {
        if (GameManager.inst.curPhase != GamePhase.Ready)
            return;

        BuildManager.inst.StartBuild(curType);
        Exit();
    }

    void OnClickSell()
    {
        if (GameManager.inst.curPhase != GamePhase.Ready)
            return;

        UIStateManager.inst.ShowPopUp("정말\n판매하시겠습니까?\n(+2코스트)",
            () =>
            {
                if (targetBarrack != null && targetBarrack.Faction == UnitFaction.Player)
                {
                    targetBarrack.Sell();
                    // UI 갱신
                    Open(curType);
                }
                else
                    InfoMassage.inst.ShowMessage(msg_CantSell);

                Exit();
            });
    }

    void OnClickUpgrade()
    {
        if (GameManager.inst.curPhase != GamePhase.Ready)
            return;

        UIStateManager.inst.ShowPopUp("미구현 기능",
            () =>{Exit(); });

        //if (targetBarrack != null && targetBarrack.Faction == UnitFaction.Player)
        //{
        //    targetBarrack.Upgrade();
        //    levelText.text = $"Lv.{targetBarrack.CurLevel}";
        //}
        //else
        //{
        //    InfoMassage.inst.ShowMessage(msg_CantUpgarade);
        //}
    }


    // 닫기 버튼 클릭 시 호출
    public void Exit()
    {
        Unsubscribe();
        curType = UnitClassType.Init;
        gameObject.SetActive(false);
    }


    // 유틸리티
    string ConvertToNameStr(UnitClassType type) => type switch
    {
        UnitClassType.Babarian => "바바리안",
        UnitClassType.Knight => "나이트",
        UnitClassType.Rogue => "로그",
        UnitClassType.Ranger => "레인저",
        UnitClassType.Mage => "메이지",

        UnitClassType.SK_Worrior => "해골 전사",
        UnitClassType.SK_Rogue => "해골 로그",
        UnitClassType.SK_Ranger => "해골 레인저",
        UnitClassType.SK_Mage => "해골 메이지",
        UnitClassType.SK_Minion => "해골 미니언",

        _ => ""
    };

    string GetClassTooltip(UnitClassType type) => type switch
    {
        UnitClassType.Babarian => "전쟁광: 한 번에 2회 공격합니다.",
        UnitClassType.Knight => "판금갑옷: 방어력을 +2 얻습니다.",
        UnitClassType.Rogue => "재빠른 몸놀림: 공격속도 +10%와 이동속도 +10%를 얻습니다.",
        UnitClassType.Ranger => "약점노출: 치명타 피해량이 +25% 증가합니다. 또한 스킬에 치명타가 발동할 수 있습니다.",
        UnitClassType.Mage => "통달: 매 초 +3의 마나를 얻습니다.",

        UnitClassType.SK_Worrior => "",
        UnitClassType.SK_Rogue => "",
        UnitClassType.SK_Ranger => "",
        UnitClassType.SK_Mage => "",
        UnitClassType.SK_Minion => "",

        _ => ""
    };

}
