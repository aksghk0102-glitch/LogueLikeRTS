using UnityEngine;

public enum TempUnitType
{
    Player = 0,
    Enemy = 1
}
public class TestGameOver : MonoBehaviour
{
    public TempUnitType curUnitType = TempUnitType.Player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider coll)
    {
        if(curUnitType == TempUnitType.Player)
        {
            if (coll.CompareTag("EnemyUnit") == true)
                Game_Mgr.Inst.IsGameOver();
        }
        else if(curUnitType == TempUnitType.Enemy)
        {
            if (coll.CompareTag("PlayerUnit") == true)
                Game_Mgr.Inst.IsGameOver();
        }
    }
}
