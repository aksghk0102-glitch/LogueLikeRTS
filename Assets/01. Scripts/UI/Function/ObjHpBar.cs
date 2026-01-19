using UnityEngine;
using UnityEngine.UI;

public class ObjHpBar : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] Image hpBar;       // 체력바
    [SerializeField] Image mpBar;       // 마나바
    [SerializeField] GameObject mpBarRoot;  // 마나바 루트
    [SerializeField] Transform cdtContainer; // 상태이상 아이콘 표시용

    IDamageable target;     // 체력바 참조
    Entity targetEntity;    // 유닛 참조 > 마나바도 동기화하기 위함
    Transform targetTr;
    Vector3 offSet = new Vector3(0f, 2.5f, 0f);

    public void SetTarget(GameObject targetObj)
    {
        targetTr = targetObj.transform;
        transform.SetParent(targetTr);

        // 컴포넌트 추출 후 타입 캐스팅
        target = targetObj.GetComponent<IDamageable>();
        
        if (target is Entity)
        {
            // 유닛인 경우
            targetEntity = (Entity)target;
            target = targetEntity;
            offSet = new Vector3(0f, 3f, 0f);
            if (mpBarRoot != null)
                mpBarRoot.SetActive(true);
        }
        else if(target is Tower)
        {
            target = targetObj.GetComponent<IDamageable>();
            offSet = new Vector3(0f, 5f, 0f);
            if (mpBarRoot != null)
                mpBarRoot.SetActive(false);
        }
        else
        {
            target = targetObj.GetComponent<IDamageable>();
            offSet = new Vector3(0f, 3.5f, 0f);
            if (mpBarRoot != null)
                mpBarRoot.SetActive(false);
        }

        // 로컬 값만 수정해서 오프셋 적용
        transform.localPosition = offSet;
    }

    private void LateUpdate()
    {
        if (targetTr == null || target == null)
            return;

        float a_CurHp = target.curHp > 0f ? target.curHp : 0f;
        hpBar.fillAmount = a_CurHp / target.maxHp;

        if (targetEntity != null)
        {
            float a_CurMana = 0;
            a_CurMana = targetEntity.curMana > 0f ? targetEntity.curMana : 0f;
            mpBar.fillAmount = targetEntity.curMana / targetEntity.MaxMana;
        }
    }
}
