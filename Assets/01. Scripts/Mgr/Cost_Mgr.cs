using UnityEngine;
using UnityEngine.UI;

public class Cost_Mgr : MonoBehaviour
{
    private int maxCost = 10;           //최대 코스트 값
    private int curCost = 0;            //현재 코스트 값

    public Text curCost_Text = null;

    [SerializeField]
    private float recoveryInterval = 5.0f;  //코스트 회복 대기시간 변수
    [SerializeField]
    private float timer = 0.0f;         //코스트 회복 시간 계산용 변수

    public int CurCost                  //현재 코스트 프러퍼티
    {
        get => curCost;

        private set
        {
            //세터로 받은 값으 0과 최대값 사이로 제한
            int newValue = Mathf.Clamp(value, 0, maxCost);

            if(curCost != newValue)
            {
                curCost = newValue;
                CostUIChanged();    //UI 갱신용
            }
        }
    }

    public static Cost_Mgr Inst;

    private void Awake()
    {
        Inst = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CurCost = 5;    //  빠른 테스트를 위해 값을 올려두었습니다.
    }

    // Update is called once per frame
    void Update()
    {
        RecoveryCostOverTime();
    }

    //코스트 자동 회복 함수
    void RecoveryCostOverTime()
    {
        timer += Time.deltaTime;

        if (timer > recoveryInterval) 
        {
            timer = 0.0f;
            CurCost += 1;
        }
    }

    //코스트 소모 함수 (외부용)
    public bool TrySpendCost(int costValue)
    {
        if (CurCost < costValue)
        {
            Debug.Log("코스트 부족");
            return false;
        }
            
        CurCost -= costValue;
        return true;
    }
    //코스트 추가 함수 (외부용)
    public void AddCost(int costValue)
    {
        CurCost += costValue;
    }
    //코스트 UI 갱신 함수
    void CostUIChanged()
    {
        if (curCost_Text == null)
            return;

        curCost_Text.text = curCost + " / 10";
    }
}
