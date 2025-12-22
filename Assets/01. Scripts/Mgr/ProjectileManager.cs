using UnityEngine;
using System.Collections.Generic;

public enum ProjKey
{

}

public class ProjectileManager : MonoBehaviour
{
    public static ProjectileManager inst;

    [System.Serializable]
    public struct ProjData
    {
        public ProjKey key;
        public GameObject prefab;
    }

    [Header("Projectiles")]
    [SerializeField] private List<ProjData> projs;

    Dictionary<ProjKey, List<Projectile>> pools = new Dictionary<ProjKey, List<Projectile>>();
    Dictionary<ProjKey, GameObject> prefabs = new Dictionary<ProjKey, GameObject>();

    void Awake()
    {
        if (inst == null)
            inst = this;
        else
            Destroy(gameObject);

        foreach(var data in projs)
        {
            if (data.prefab == null || prefabs.ContainsKey(data.key))
                continue;

            prefabs[data.key] = data.prefab;
            //여기부터 이어하기
        }
    }
}
