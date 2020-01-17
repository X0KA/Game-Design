using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CapLevels : MonoBehaviour
{
    Text LevelInput;
    InputField input;
    // Start is called before the first frame update
    void Start()
    {
        LevelInput = transform.Find("Text").GetComponent<Text>();
        input = GetComponent<InputField>();
    }

    // Update is called once per frame
    void Update()
    { 
        string a = input.text;
        if (a.Length<=0)
        {
            a = 0.ToString();
        }
   
        int lvl = int.Parse(a);
        if (lvl > 10) { lvl = 10; input.text= lvl.ToString();}
        if (lvl <= 0) { lvl = 1; input.text = lvl.ToString(); }



    }
}
