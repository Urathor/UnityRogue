using UnityEngine.UI;
using UnityEngine;

public class MonsterStat : MonoBehaviour
{
    public Image FillImage;
    public Text Label;


    public void SetFill(float amt)
    {
        FillImage.fillAmount = amt;
    }

    public void SetLabel(string name, Color clr)
    {
        Label.text = $"{name}";
        Label.color = clr;
    }
}
