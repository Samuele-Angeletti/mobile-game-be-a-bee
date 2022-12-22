using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIGameOver : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI metersDone;
    [SerializeField] TextMeshProUGUI honeyDone;
    [SerializeField] TextMeshProUGUI finalFlockDone;
    [SerializeField] TextMeshProUGUI bossesKilled;
    [SerializeField] TextMeshProUGUI enemiesKilled;
    [SerializeField] TextMeshProUGUI bombsUsed;
    [SerializeField] TextMeshProUGUI invulnerabilityUsed;
    public void FillUpStatistics()
    {
        metersDone.text = $"{PlayerStatistics.CurrentMeters}";
        honeyDone.text = $"{PlayerStatistics.CurrentHoney}";
        finalFlockDone.text = $"{PlayerStatistics.CurrentFlock}";
        bossesKilled.text = $"{PlayerStatistics.CurrentBossesKilled}";
        enemiesKilled.text = $"{PlayerStatistics.CurrentEnemiesKilled}";
        bombsUsed.text = $"{PlayerStatistics.CurrentBombsUsed}";
        invulnerabilityUsed.text = $"{PlayerStatistics.CurrentInvulnerability}";
    }
}
