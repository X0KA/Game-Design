/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleOverWindow : MonoBehaviour {

    private static BattleOverWindow instance;

    private void Awake() {
        instance = this;
        Hide();
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    private void Show(string winnerString) {
        gameObject.SetActive(true);

        transform.Find("winnerText").GetComponent<Text>().text = winnerString;
    }

    public static void Show_Static(string winnerString) {
        instance.Show(winnerString);
    }

}
