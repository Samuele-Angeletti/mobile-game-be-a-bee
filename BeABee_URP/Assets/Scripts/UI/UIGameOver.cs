using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGameOver : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI metersDone;
    [SerializeField] TextMeshProUGUI honeyDone;
    [SerializeField] TextMeshProUGUI finalFlockDone;
    [SerializeField] TextMeshProUGUI bossesKilled;
    [SerializeField] TextMeshProUGUI enemiesKilled;
    [SerializeField] TextMeshProUGUI bombsUsed;
    [SerializeField] TextMeshProUGUI invulnerabilityUsed;
    [SerializeField] Slider pollenSlider;
    [SerializeField] TextMeshProUGUI honeyBoxes;
    [SerializeField] Button getHoneyButton;
    private int currentBoxes;
    public void FillUpStatistics()
    {
        metersDone.text = $"{PlayerStatistics.CurrentMeters}";
        honeyDone.text = $"{PlayerStatistics.CurrentHoney}";
        finalFlockDone.text = $"{PlayerStatistics.CurrentFlock}";
        bossesKilled.text = $"{PlayerStatistics.CurrentBossesKilled}";
        enemiesKilled.text = $"{PlayerStatistics.CurrentEnemiesKilled}";
        bombsUsed.text = $"{PlayerStatistics.CurrentBombsUsed}";
        invulnerabilityUsed.text = $"{PlayerStatistics.CurrentInvulnerability}";

        pollenSlider.value = PlayerStatistics.LastTotalPollen / 100;
        honeyBoxes.text = $"x {PlayerStatistics.LastTotalBoxOfHoney}";
        currentBoxes = PlayerStatistics.LastTotalBoxOfHoney;
        StartCoroutine(SliderGrowCoroutine());
    }

    private IEnumerator SliderGrowCoroutine()
    {
        for (int i = 0; i < PlayerStatistics.BoxOfHoney; i++)
        {
            while(true)
            {
                yield return new WaitForEndOfFrame();
                pollenSlider.value += 0.01f;
                if (pollenSlider.value >= 1)
                    break;
            }
            currentBoxes++;
            honeyBoxes.text = $"x {currentBoxes}";
            pollenSlider.value = 0;
        }

        while (true)
        {
            yield return new WaitForEndOfFrame();
            pollenSlider.value += 0.01f;
            if (pollenSlider.value * 100 >= PlayerStatistics.TotalPollen)
                break;
        }


        getHoneyButton.interactable = currentBoxes > 0;
    }
}
