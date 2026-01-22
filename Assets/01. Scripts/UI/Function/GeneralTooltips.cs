using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GeneralTooltips : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI contents;
    RectTransform rect;
    Vector2 offset = new Vector2(15f, -15f);
    
    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public void SetText(string tText, string cText)
    {
        gameObject.SetActive(false);
        title.text = tText;
        contents.text = cText;

        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);

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

        rect.position = mousePos + offset;

        // 화면 밖으로 나가는 경우 방지
    }
}
