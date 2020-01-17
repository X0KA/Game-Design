using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ButtonAvailable : MonoBehaviour
{
    public GameObject NPC;
    public int identifier;
    // Start is called before the first frame update
    private Stats charStats;
    private Button button;
   
    void Start()
    {
        charStats = NPC.GetComponent<Stats>();
        button = this.GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        if (identifier < charStats.AbleToSkill())
        {
            button.interactable = true;
        }
        else
            button.interactable = false;

    }
}
