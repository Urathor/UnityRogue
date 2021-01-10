using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatDisplay : MonoBehaviour
{
    public Image StatImage;
    public Text StatText;

    public void SetIcon(Sprite icon)
    {
        StatImage.sprite = icon;
    }

    public void SetText(string txt)
    {
        StatText.text = txt;
    }
}
