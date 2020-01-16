using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetButton : MonoBehaviour
{
    public GameObject button;
    private bool active=false;
    // Start is called before the first frame update
    void Start()
    {
        button.SetActive(active);
    }

    // Update is called once per frame
    void Update()
    {  
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            active = !active;
            button.SetActive(active);
        }
    }
}
