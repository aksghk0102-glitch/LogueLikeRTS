using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BuildCard : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] UnitClassType unitType;
    [SerializeField] Image thisImage;
    [SerializeField] BuildManager manager;

    public void OnBeginDrag(PointerEventData eventData)
    {
        manager.StartBuild(unitType);

        if (thisImage != null)
            thisImage.color = new Color(1, 1, 1, 0.7f);
    }

    public void OnDrag(PointerEventData eventData)
    {

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (thisImage != null)
            thisImage.color = new Color(1, 1, 1, 1f);
    }
}
