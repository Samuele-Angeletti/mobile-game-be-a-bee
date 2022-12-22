using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIShopMenu : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI totalMetersText;
    [SerializeField] TextMeshProUGUI totalHoneyText;

    public void LoadStatistics()
    {
        totalMetersText.text = $"{PlayerStatistics.TotalMeters}";
        totalHoneyText.text = $"{PlayerStatistics.TotalHoney}";
    }
}
