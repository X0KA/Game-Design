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
using TMPro;
using CodeMonkey.Utils;

public class TextScript : MonoBehaviour
{

    // Create a Damage Popup
    public static TextScript Create(Vector3 position, string text, int Size, bool dynamic = false, GameObject parent = null)
    {
        Transform damagePopupTransform = null;
        if (parent != null)
        {
            damagePopupTransform = Instantiate(GameAssets.i.pfTextScript, position, Quaternion.identity, parent.transform);
        }
        else
        {
            damagePopupTransform = Instantiate(GameAssets.i.pfTextScript, position, Quaternion.identity);
        }

        TextScript damagePopup = damagePopupTransform.GetComponent<TextScript>();
        damagePopup.Setup(text, Size, dynamic);

        return damagePopup;
    }

    private static int sortingOrder = 10000;

    private const float DISAPPEAR_TIMER_MAX = 2.1f;

    private TextMeshPro textMesh;
    private float disappearTimer;
    private Color textColor;
    private Vector3 moveVector;
    private bool isDynamic;

    private void Awake()
    {
        textMesh = transform.GetComponent<TextMeshPro>();
    }

    public void Setup(string Text, int Size, bool dynamic)
    {
        textMesh.SetText(Text);

        isDynamic = dynamic;
        textMesh.fontSize = Size;
        textColor = Color.white;
            
        
        textMesh.color = textColor;
        disappearTimer = DISAPPEAR_TIMER_MAX;

        sortingOrder++;
        textMesh.sortingOrder = sortingOrder;

        moveVector = new Vector3(.7f, 1) * 60f;
    }

    private void Update()
    {

        if (isDynamic)
        {
            transform.position += moveVector * Time.deltaTime;
            moveVector -= moveVector * 8f * Time.deltaTime;

            if (disappearTimer > DISAPPEAR_TIMER_MAX * .5f)
            {
                // First half of the popup lifetime
                float increaseScaleAmount = 0.1f;
                transform.localScale += Vector3.one * increaseScaleAmount * Time.deltaTime;
            }
            else
            {
                // Second half of the popup lifetime
                float decreaseScaleAmount = 0.1f;
                transform.localScale -= Vector3.one * decreaseScaleAmount * Time.deltaTime;
            }

            disappearTimer -= Time.deltaTime;
            if (disappearTimer < 0)
            {
                // Start disappearing
                float disappearSpeed = 3f;
                textColor.a -= disappearSpeed * Time.deltaTime;
                textMesh.color = textColor;
                if (textColor.a < 0)
                {
                    Destroy(gameObject);
                }
            }
        }

    }

}
