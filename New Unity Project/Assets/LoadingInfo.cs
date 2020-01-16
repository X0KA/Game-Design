using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingInfo : MonoBehaviour
{

    public enum EnemyTypes
    {
        Guard,
        Damicist,
        Protector,
        Amadea
    }

    public GameObject Admon;
    public GameObject Florence;
    public GameObject Claudio;

    public EnemyTypes[] EnemyList;

    private void Start()
    {
        EnemyList = new EnemyTypes[3];
        EnemyList[0] = EnemyTypes.Guard;
        EnemyList[1] = EnemyTypes.Guard;
        EnemyList[2] = EnemyTypes.Guard;
    }
    public bool PlayerVsBot;
    // Start is called before the first frame update

    public void SetPlayerVsBot()
    {
        PlayerVsBot = true;
    }
    public void setBotvsBot()
    {
        PlayerVsBot = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetAdmonLvl(string Level)
    {
        int lvl = int.Parse(Level);
        
        Admon.GetComponent<Stats>().AdmonSetStatsbyLevel(lvl);
    }

    public void SetClaudioLvl(string Level)
    {
        int lvl = int.Parse(Level);

        Claudio.GetComponent<Stats>().AdmonSetStatsbyLevel(lvl);
    }
    public void SetFlorenceLvl(string Level)
    {
        int lvl = int.Parse(Level);

        Florence.GetComponent<Stats>().AdmonSetStatsbyLevel(lvl);
    }

    public void CheckMaxandMinLevel(string Level)
    {

        int lvl = int.Parse(Level);
        if (lvl > 10) lvl = 10;
        if (lvl <= 0) lvl = 1;

        Level = lvl.ToString();
        
    }

    public void SetEnemeyOne(int index)
    {
        EnemyList[0] = (EnemyTypes)index;
    }
    public void SetEnemeyTwo(int index)
    {
        EnemyList[1] = (EnemyTypes)index;
    }
    public void SetEnemeyThree(int index)
    {
        EnemyList[2] = (EnemyTypes)index;
    }


}
