using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class XPBarManager : MonoBehaviour
{
    public Image Bar;
    public TextMeshProUGUI Amount;
    public TextMeshProUGUI Level;
    
    void Start()
    {
        UpdateXPBar();
    }

    public void UpdateXPBar()
    {
        Amount.text = PlayerController.Instance.XP.ToString() + " / " + PlayerController.Instance.XPNeeded.ToString();
        Level.text = PlayerController.Instance.Level.ToString();
        Bar.fillAmount = (float)PlayerController.Instance.XP / (float)PlayerController.Instance.XPNeeded;
    }
}
