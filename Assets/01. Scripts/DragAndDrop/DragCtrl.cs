using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// 카드 UI에 붙여 사용.
// 드래그 앤 드랍으로 몬스터 스폰을 구현합니다.

public class DragCtrl : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] UnitData unitData;
    int SpawnIndex => unitData.unitUniqIndex;

    bool isValidDrag = false;
    Vector3 originPos;

    void Awake()
    {
        originPos = transform.position;
    }

    public void OnBeginDrag(PointerEventData eventData)        // 드래그 시작 시
    {
        isValidDrag = false;        // 클릭만 했을 때는 조작되지 않도록
    }

    public void OnDrag(PointerEventData eventData)              // 드래그 도중
    {
        // 마우스 위치 확인
        bool isOverUI = IsPointerOverUIObject();

        if (!isOverUI)      // 마우스 아래 UI가 없으면 미리보기 생성
        {
            if (!isValidDrag)
            {
                isValidDrag = true;

                // 미리보기 생성 및 표시
                Game_Mgr.Inst.SpawnGhost(eventData.position, SpawnIndex);
            }

            // 미리보기 위치 업데이트
            Game_Mgr.Inst.OnDragUpdate(eventData.position, SpawnIndex);
        }
        else               // 마우스 아래 UI가 감지되면 미리보기 취소
        {
            if (isValidDrag)
            {
                isValidDrag = false;

                // 미리보기 비활성화
                Game_Mgr.Inst.EndDrag();
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)           // 드래그 종료
    {
        if (isValidDrag)
        {
            // 드롭 확정
            Game_Mgr.Inst.Drop(eventData.position, SpawnIndex);
        }

        Game_Mgr.Inst.EndDrag();
        isValidDrag = false;
    }

    public static bool IsPointerOverUIObject() //UGUI의 UI들이 먼저 피킹되는지 확인하는 함수
    {
        PointerEventData a_EDCurPos = new PointerEventData(EventSystem.current);

#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID)

			List<RaycastResult> results = new List<RaycastResult>();
			for (int i = 0; i < Input.touchCount; ++i)
			{
				a_EDCurPos.position = Input.GetTouch(i).position;  
				results.Clear();
				EventSystem.current.RaycastAll(a_EDCurPos, results);
                if (0 < results.Count)
                    return true;
			}

			return false;
#else
        a_EDCurPos.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(a_EDCurPos, results);
        return (0 < results.Count);
#endif
    }//public bool IsPointerOverUIObject() 
}
