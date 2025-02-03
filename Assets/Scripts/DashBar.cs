using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashBar : MonoBehaviour
{
    public Image bar;

    void Update()
    {
        UpdateDashBar();
    }

    void UpdateDashBar()
    {
        float DashPercent = 1 - ( (float)(PlayerController.Instance.dashCooldown - (Time.time - PlayerController.Instance.lastDashTime)) / PlayerController.Instance.dashCooldown);

        DashPercent = Mathf.Clamp01(DashPercent);

        bar.fillAmount = DashPercent;
    }
}
