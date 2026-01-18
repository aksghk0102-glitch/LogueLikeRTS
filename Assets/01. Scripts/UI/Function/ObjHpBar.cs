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


}
