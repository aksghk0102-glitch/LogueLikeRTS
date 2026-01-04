using UnityEngine;
using TMPro;

public class TooltipUI : MonoBehaviour
{
    public static TooltipUI Inst;

    public GameObject panel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descText;

    private void Awake()
    {
        if (Inst == null)
            Inst = this;
        else
            Destroy(gameObject);
    }

    void Show(SkillData data)
    {
        panel.SetActive(true);
        nameText.text = data.Name;

        string finalDesc = data.Desc;
        foreach(var p in data.Params)
        {
            finalDesc = finalDesc
                .Replace("{" + p.Key + "}", p.Value.ToString());
        }
        descText.text = finalDesc;
    }

    public void Hide()
    {
        panel.SetActive(false);
    }
}
