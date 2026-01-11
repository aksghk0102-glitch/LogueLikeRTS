using UnityEngine;
using TMPro;
using DG.Tweening;

public class InfoMassage : MonoBehaviour
{
    public static InfoMassage inst;
    [SerializeField] TextMeshProUGUI infoText;

    float durTime = 0.3f;
    float showTime = 3f;

    private void Awake()
    {
        if (inst == null)
            inst = this;
        else
            Destroy(gameObject);

        if (infoText != null)
            infoText.gameObject.SetActive(false);
    }

    public void ShowPerMessage(string msg)
    {
        if (infoText == null) return;

        //infoText.DOKill();
        //infoText.text = msg;
        //infoText.alpha = 1f;
        //infoText.gameObject.SetActive(true);

        PlayAppearAnim(msg);
    }

    public void ShowMessage(string msg)
    {
        if (infoText == null) return;

        PlayAppearAnim(msg);

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(showTime);                 // 팝업 시간 설정
        seq.Append(infoText.DOFade(0f, durTime));
        seq.Join(infoText.transform
            .DOScale(0.8f, durTime)
            .SetEase(Ease.InBack));
        seq.OnComplete(() => infoText.gameObject.SetActive(false));
    }

    // 나타날 때 연출
    void PlayAppearAnim(string msg)
    {
        infoText.DOKill();
        infoText.transform.DOKill();

        infoText.text = msg;
        infoText.gameObject.SetActive(true);

        // 초기화
        infoText.alpha = 0f;
        infoText.transform.localScale = Vector3.one * 0.8f;

        infoText.DOFade(1f, durTime)
            .SetEase(Ease.OutBack);
    }
}
