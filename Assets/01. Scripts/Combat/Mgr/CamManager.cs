using UnityEngine;

public class CamManager : MonoBehaviour
{
    [Header("=== Base Settings ===")]   // 카메라 범위 셋팅
    [SerializeField] float moveSpeed = 15f;
    [SerializeField] float edgeSize = 20f;          // 화면 끝 감지 범위
    [SerializeField] Vector2 limitMin = new Vector2(-50, -50); // x,z 최소
    [SerializeField] Vector2 limitMax = new Vector2(50, 50);   // x,z 최대

    [Header("=== Zoom ===")]        // 휠로 줌인아웃
    [SerializeField] float minHeight = 5f;          // 최소 높이
    [SerializeField] float maxHeight = 20f;         // 최대 높이
    [SerializeField] float heightSpeed = 15f;       // 휠 감도
    [SerializeField] float currentHeight = 13f;     // 초기 높이

    [Header("=== Rotate ===")]
    [SerializeField] float rotationSpeed = 3f;      // 회전 속도
    [SerializeField] float minPitch = -45f;         // 너무 아래로 안 내려가게
    [SerializeField] float maxPitch = 75f;          // 너무 위로 안 올라가게

    private Camera mainCam;
    private float yaw = 0f;    // 좌우 회전
    private float pitch = 55f; // 상하 각도 (초기값)

    private void Awake()
    {
        mainCam = Camera.main;
        if (mainCam == null)
        {
            Debug.LogError("Main Camera를 찾을 수 없습니다!");
        }

        // 초기 위치 & 회전 적용
        //ApplyCameraTransform();
    }

    private void Update()
    {
        HandleEdgeMove();      // 기존 가장자리 이동
        HandleHeightZoom();    // 휠로 높이 조절
        HandleRotation();      // 우클릭 드래그 회전

        // 최종 위치·회전 적용
        ApplyCameraTransform();
    }

    void HandleEdgeMove()
    {
        Vector3 pos = transform.position; // CamManager가 부모라면 transform.position 사용
        Vector3 mousePos = Input.mousePosition;

        // 상하 (Z축)
        if (mousePos.y >= Screen.height - edgeSize) pos.z += moveSpeed * Time.deltaTime;
        else if (mousePos.y <= edgeSize) pos.z -= moveSpeed * Time.deltaTime;

        // 좌우 (X축)
        if (mousePos.x >= Screen.width - edgeSize) pos.x += moveSpeed * Time.deltaTime;
        else if (mousePos.x <= edgeSize) pos.x -= moveSpeed * Time.deltaTime;

        // 제한 적용 (x, z만)
        pos.x = Mathf.Clamp(pos.x, limitMin.x, limitMax.x);
        pos.z = Mathf.Clamp(pos.z, limitMin.y, limitMax.y);

        transform.position = pos;
    }

    void HandleHeightZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            currentHeight -= scroll * heightSpeed; // 위로 돌리면 높이 ↑ (직관적)
            currentHeight = Mathf.Clamp(currentHeight, minHeight, maxHeight);
        }
    }

    void HandleRotation()
    {
        if (Input.GetMouseButton(1)) // 우클릭 눌린 상태
        {
            float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
            float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

            yaw += mouseX;
            pitch -= mouseY; // 상하 반전 (자연스러움)
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }
    }

    void ApplyCameraTransform()
    {
        // 회전 적용
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        // 높이 기반 오프셋 (높을수록 뒤로 더 물림)
        float distance = currentHeight * 1.5f; // 높이에 비례해서 거리 조절
        Vector3 offset = rotation * new Vector3(0, 0, -distance);

        // 최종 위치 = 바닥 위치 + 오프셋
        Vector3 targetPos = transform.position + Vector3.up * currentHeight;
        mainCam.transform.position = targetPos + offset;

        // 카메라가 항상 타겟(바닥 위치 + 약간 위)을 바라보게
        mainCam.transform.LookAt(targetPos + Vector3.up * 2f);
    }

    // 필요시 외부에서 호출 가능한 메서드
    public void ResetCamera()
    {
        yaw = 0f;
        pitch = 30f;
        currentHeight = 10f;
        transform.position = Vector3.zero; // 필요시 초기 위치로
    }
}