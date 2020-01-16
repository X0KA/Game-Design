using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingInfo : MonoBehaviour
{

    public GameObject Admon;
    public GameObject Florence;
    public GameObject Claudio;


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

}
