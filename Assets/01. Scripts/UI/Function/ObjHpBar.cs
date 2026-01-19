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
        targetEntity = targetObj.GetComponent<Entity>();
        
        if(targetEntity != null)
        {
            // 유닛인 경우
            target = targetEntity;
            offSet = new Vector3(0f, 3f, 0f);
            if (mpBarRoot != null)
                mpBarRoot.SetActive(true);
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

        if (target.curHp > 0)
            hpBar.fillAmount = target.curHp / target.maxHp;

        if (targetEntity != null && targetEntity.MaxMana > 0)
            mpBar.fillAmount = targetEntity.curMana / targetEntity.MaxMana;
    }
}
