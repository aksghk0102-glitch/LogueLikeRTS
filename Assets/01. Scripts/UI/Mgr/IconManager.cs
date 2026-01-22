using UnityEngine;
using System.Collections.Generic;

public class IconManager : MonoBehaviour
{
    public static IconManager inst;

    [Header("Unit Icons")]
    [SerializeField] Sprite defalutIcon;
    [SerializeField] List<IconMappedData> unitIconList;
    
    Dictionary<UnitClassType, Sprite> iconDict = new Dictionary<UnitClassType, Sprite>();
    [System.Serializable]
    public class IconMappedData
    {
        public UnitClassType type;
        public Sprite icon;
    }

    private void Awake()
    {
        if(inst == null)
            inst = this;

        foreach(var ui in unitIconList)
        {
            if(!iconDict.ContainsKey(ui.type))
                iconDict.Add(ui.type, ui.icon);
        }
    }

    public Sprite GetUnitIcon(int id)
    {
        if(id <= 0)
            return defalutIcon;

        UnitClassType type = (UnitClassType)(id / 10000);   // 생성 시점 *10000 하므로 역산.

        if(iconDict.TryGetValue(type, out Sprite targetIcon))
            return targetIcon;

        return defalutIcon;

    }
}
