using UnityEngine;
using TMPro;
using DG.Tweening;

public class GoldGetText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] float moveDist = 2f;
    [SerializeField] float duration = 2f;
    [SerializeField] Color color;

    Sequence textSeq;
    RectTransform rectTrans;



    void Awake()
    {
        rectTrans = GetComponent<RectTransform>();
    }
    public void Init(string contents)
    {
        textSeq?.Kill();

        text.text = contents;
        text.alpha = 255f;
        text.color = color;

        // 위치 초기화
        rectTrans.anchoredPosition = Vector2.zero;
        gameObject.SetActive(true);

        textSeq = DOTween.Sequence()
            .Append(rectTrans.DOAnchorPosY(moveDist, duration))
            .SetEase(Ease.OutQuad)
            .Join(text.DOFade(0, duration).SetEase(Ease.InQuad))
            .OnComplete(() => gameObject.SetActive(false));
    }

    
}
