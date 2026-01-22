using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GeneralTooltips : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI contents;
    [SerializeField] RectTransform contentsParents;     // 툴팁 아래 중간 부모 개체
    RectTransform rect;
    Vector2 offset = new Vector2(15f, -15f);
    
    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public void SetText(string tText, string cText)
    {
        gameObject.SetActive(true);
        title.text = tText;
        contents.text = cText;

        // 레이아웃 갱신
        if (contentsParents != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentsParents);

        UpdatePosition();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (gameObject.activeSelf)
            UpdatePosition();
    }

    void UpdatePosition()
    {
        Vector2 mousePos = Input.mousePosition;
        Vector2 targetPos = mousePos + offset;

        // 화면 밖으로 나가는 경우 방지 : Pivot (0, 1) 기준 - 좌상단 정점
        float ttWidth = contentsParents.rect.width;
        float ttHeight = contentsParents.rect.height;

        float minX = 0;
        float maxX = Screen.width - ttWidth;
        float minY = ttHeight;
        float maxY = Screen.height;

        targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
        targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);

        rect.position = targetPos;
    }
}
