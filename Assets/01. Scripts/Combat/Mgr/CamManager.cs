using UnityEngine;
using UnityEngine.EventSystems;
public class CamManager : MonoBehaviour
{
    [Header("=== Base Settings ===")]   // 카메라 범위 셋팅
    [SerializeField] float moveSpeed = 15f;
    [SerializeField] float edgeSize = 50f;          // 화면 끝 감지 범위
    [SerializeField] Vector2 limitMin = new Vector2(-50, -50); // x,z 최소
    [SerializeField] Vector2 limitMax = new Vector2(50, 50);   // x,z 최대

    [Header("=== Zoom ===")]        // 휠로 줌인아웃
    [SerializeField] float minHeight = 5f;          // 최소 높이
    [SerializeField] float maxHeight = 20f;         // 최대 높이
    [SerializeField] float heightSpeed = 15f;       // 휠 감도

    [Header("=== Rotation===")]
    [SerializeField] float rotationSpeed = 3f;      // 회전 속도
    [SerializeField] float currentHeight = 15f;
    [SerializeField] float minPitch = -45f;         // 너무 아래로 안 내려가게
    [SerializeField] float maxPitch = 75f;          // 너무 위로 안 올라가게
    
    private float yaw = 0f;    // 좌우 회전
    private float pitch = 55f; // 상하 각도 (초기값)

    private Camera mainCam;

    private void Awake()
    {
        mainCam = Camera.main;
        if (mainCam == null)
        {
            Debug.LogError("Main Camera를 찾을 수 없습니다!");
        }

        // 초기 회전 및 높이 적용
        mainCam.transform.position = new Vector3(mainCam.transform.position.x, currentHeight, mainCam.transform.position.z);
        mainCam.transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    private void LateUpdate()
    {
        // UI 클릭 중일 때는 레이캐스트(선택) 무시
        if (EventSystem.current.IsPointerOverGameObject()) return;

        HandleRotation();   // 우클릭 드래그 회전 로직 복구
        HandleEdgeMove();   // 카메라 시선 기준 이동 (수정됨)
        HandleHeightZoom(); // 휠로 높이 조정

        // 최종 회전 적용
        mainCam.transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    void HandleEdgeMove()
    {
        Vector3 moveDir = Vector3.zero;
        Vector3 mousePos = Input.mousePosition;

        // 캐싱된 카메라의 현재 yaw를 기준으로 수평 이동 방향 계산
        Vector3 forward = Quaternion.Euler(0, yaw, 0) * Vector3.forward;
        Vector3 right = Quaternion.Euler(0, yaw, 0) * Vector3.right;

        if (mousePos.y >= Screen.height - edgeSize) moveDir += forward;
        else if (mousePos.y <= edgeSize) moveDir -= forward;

        if (mousePos.x >= Screen.width - edgeSize) moveDir += right;
        else if (mousePos.x <= edgeSize) moveDir -= right;

        // WASD 키보드 입력 체크
        if (Input.GetKey(KeyCode.W)) moveDir += forward;
        if (Input.GetKey(KeyCode.S)) moveDir -= forward;
        if (Input.GetKey(KeyCode.A)) moveDir -= right;
        if (Input.GetKey(KeyCode.D)) moveDir += right;

        if (moveDir != Vector3.zero)
        {
            // 캐싱된 카메라(mainCam)의 월드 좌표를 직접 수정
            Vector3 nextPos = mainCam.transform.position + (moveDir.normalized * moveSpeed * Time.deltaTime);

            nextPos.x = Mathf.Clamp(nextPos.x, limitMin.x, limitMax.x);
            nextPos.z = Mathf.Clamp(nextPos.z, limitMin.y, limitMax.y);

            mainCam.transform.position = new Vector3(nextPos.x, currentHeight, nextPos.z);
        }
        else
        {
            // 이동이 없어도 줌에 따른 높이 변화는 직접 반영
            mainCam.transform.position = new Vector3(mainCam.transform.position.x, currentHeight, mainCam.transform.position.z);
        }
    }

    void HandleHeightZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            currentHeight -= scroll * heightSpeed;
            currentHeight = Mathf.Clamp(currentHeight, minHeight, maxHeight);
        }
    }

    void HandleRotation()
    {
        // 우클릭 드래그 시 yaw(좌우)와 pitch(상하)를 조절
        if (Input.GetMouseButton(1))
        {
            yaw += Input.GetAxis("Mouse X") * rotationSpeed;
            pitch -= Input.GetAxis("Mouse Y") * rotationSpeed;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }
    }

}