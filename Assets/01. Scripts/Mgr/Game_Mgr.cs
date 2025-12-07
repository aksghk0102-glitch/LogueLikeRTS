using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState
{
    GS_IsPlaying = 0,
    GS_GameOver,
    GS_IsPause
}
public class Game_Mgr : MonoBehaviour
{
    [SerializeField]
    GameObject Panel_GameOver = null; 
    [SerializeField]
    GameObject Panel_Pause = null;
    [SerializeField]
    GameObject Panel_Upgrade = null;
    [SerializeField]
    Button ReStartBtn = null;
    [SerializeField]
    Button IsPauseBtn = null;
    [SerializeField]
    Button ReleasePauseBtn = null;
    [SerializeField]
    Button UpgradeBtn = null;
    [SerializeField]
    Button CloseUpgradeBtn = null;

    public GameState gameState = GameState.GS_IsPlaying;

    public static Game_Mgr Inst = null;
    private void Awake()
    {
        Inst = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Time.timeScale = 1.0f;
        Panel_GameOver.gameObject.SetActive(false);
        Panel_Pause.gameObject.SetActive(false);
        Panel_Upgrade.gameObject.SetActive(false);
        gameState = GameState.GS_IsPlaying;

        if (ReStartBtn != null)
            ReStartBtn.onClick.AddListener(ReStart);
        if (IsPauseBtn != null)
            IsPauseBtn.onClick.AddListener(Pause);
        if (ReleasePauseBtn != null)
            ReleasePauseBtn.onClick.AddListener(ReleasePause);
        if (UpgradeBtn != null)
            UpgradeBtn.onClick.AddListener(UpgradeClick);
        if (CloseUpgradeBtn != null)
            CloseUpgradeBtn.onClick.AddListener(CloseUpgradeClick);

        // 배치 가능 영역 레이어 마스크 캐싱
        placement_LM = 1 << LayerMask.NameToLayer("PLACEMENT");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            IsGameOver();
        }
    }

    public void IsGameOver()
    {
        if (gameState != GameState.GS_IsPlaying)
            return;

        gameState = GameState.GS_GameOver;

        //승패 판정 ==> 

        Panel_GameOver.gameObject.SetActive(true);
        Time.timeScale = 0.0f;
    }

    public void Pause()
    {
        if (gameState != GameState.GS_IsPlaying)
            return;

        gameState = GameState.GS_IsPause;

        Panel_Pause.gameObject.SetActive(true);
        Time.timeScale = 0.0f;
    }

    public void ReleasePause()
    {
        if (gameState != GameState.GS_IsPause)
            return;

        gameState = GameState.GS_IsPlaying;

        Panel_Pause.gameObject.SetActive(false);
        Time.timeScale = 1.0f;
    }

    private void ReStart()
    {
        SceneManager.LoadScene("GameScene");
    }

    private void UpgradeClick()
    {
        if (Time.timeScale == 0.0f)
            return;
        Time.timeScale = 0.0f;
        Panel_Upgrade.gameObject.SetActive(true);
        SceneManager.LoadSceneAsync("UpgradeUnit", LoadSceneMode.Additive);
    }

    private void CloseUpgradeClick()
    {
        Time.timeScale = 1.0f;
        Panel_Upgrade.gameObject.SetActive(false);
        SceneManager.UnloadSceneAsync("UpgradeUnit");
    }


    #region 유닛 미리보기 시스템 표시용

    // 표시 중인 미리보기(고스트) 오브젝트
    GameObject curGhostObj = null;
    int curIndex = -1;
    Renderer[] ghostRenderer = null;  // 고스트의 렌더러
    bool isPlaceable = false;
    
    // 배치 가능 영역 탐지 레이어
    LayerMask placement_LM;
    [Header("placement boundary")]
    [SerializeField] GameObject placementBoundary;
    
    public void SpawnGhost(Vector2 screenPos, int index)
    {
        if (curGhostObj != null)
            EndDrag();

        // 미리 보기 오브젝트 생성 및 초기화
        curGhostObj = UnitSpawn_Mgr.inst.GetGhost(index);
        curIndex = index;
        ghostRenderer = curGhostObj.GetComponentsInChildren<Renderer>();

        // 미리보기 영역 표시
        if(placementBoundary != null)
            placementBoundary.SetActive(true);

        OnDragUpdate(screenPos, index);
    }

    public void OnDragUpdate(Vector2 screenPos, int index)
    {
        if (curGhostObj == null)
            return;

        // 무한대의 평면을 임의로 생성 > 미리보기 오브젝트가 마우스 트래킹 하도록
        Plane tempPlane = new Plane(Vector3.up, Vector3.zero);  // y=0 위치
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        float dis;

        // 배치 가능 영역 검사용 변수
        bool a_IsPlaceable = false;
        Vector3 ghostPos = curGhostObj.transform.position;

        if (tempPlane.Raycast(ray, out dis))
        {
            // 위치 업데이트
            Vector3 worldPos = ray.GetPoint(dis);
            curGhostObj.transform.position = worldPos;

            // 배치 가능 여부 검사
            if(Physics.Raycast(ghostPos + Vector3.up * 1f, Vector3.down,
                out RaycastHit hit, 2f, placement_LM))  // 마우스 위치에서 1f 띄운 후 아래로 2f 검사
            {
                a_IsPlaceable = true;
            }

        }

        // 배치 가능한지 표시
        if(ghostRenderer != null && ghostRenderer.Length > 0)
        {
             Color targetColor = isPlaceable ?
                ColorDefine.ValidColor : ColorDefine.InvalidColor;

            foreach (Renderer renderer in ghostRenderer)
            {
                renderer.material.SetColor("_Color", targetColor);
            }
        }

        // 플래그 갱신
        isPlaceable = a_IsPlaceable;
    }
    
    public void EndDrag()
    {
        if (curGhostObj != null)
        {
            UnitSpawn_Mgr.inst.ReturnGhost(curGhostObj, curIndex);
            curGhostObj = null;
            curIndex = -1;
        }

        if(placementBoundary != null)
        {
            placementBoundary.SetActive(false);
        }
    }

    public void Drop(Vector2 screenPos, int index)
    {
        // 배치 유효성 검사
        if(!isPlaceable)
        {
            EndDrag();
            return;
        }

        // 스폰 위치 설정
        Vector3 spawnPos = curGhostObj.transform.position;

        //유닛 스폰
        UnitSpawn_Mgr.inst.SpawnUnit(curIndex, UnitFaction.Player, spawnPos);
        EndDrag();
    }

    #endregion

}
