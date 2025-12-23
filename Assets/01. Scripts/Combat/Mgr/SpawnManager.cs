using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("For Test")]
    public UnitDataSO data;
    public GameObject testPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            GameObject go = Instantiate(testPrefab, new Vector3(-3, 1, 0), Quaternion.identity);
            Entity e = go.GetComponent<Entity>();

            if (e != null)
                e.InitEntity(data.stats, UnitFaction.Player);
        }


        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            GameObject go = Instantiate(testPrefab, new Vector3(3, 1, 0), Quaternion.identity);
            Entity e = go.GetComponent<Entity>();

            if (e != null)
                e.InitEntity(data.stats, UnitFaction.Enemy);
        }
    }
}
