using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerStatistics
{
    // total
    public static int TotalMeters 
    { 
        get; private set; 
    }
    public static int TotalHoney 
    { 
        get; private set; 
    }
    public static int TotalFlock
    {
        get; private set;
    }
    public static int TotalBossesKilled
    {
        get; private set;
    }
    public static int TotalEnemiesKilled
    {
        get; private set;
    }
    public static int TotalBombsUsed
    {
        get; private set;
    }
    public static int TotalInvulnerability
    {
        get; private set;
    }

    // last session
    public static int CurrentMeters
    {
        get; private set;
    }
    public static int CurrentHoney
    {
        get; private set;
    }
    public static int CurrentFlock
    {
        get; private set;
    }
    public static int CurrentBossesKilled
    {
        get; private set;
    }
    public static int CurrentEnemiesKilled
    {
        get; private set;
    }
    public static int CurrentBombsUsed
    {
        get; private set;
    }
    public static int CurrentInvulnerability
    {
        get; private set;
    }

    public static void SetStatistics(Statistics stats)
    {
        CurrentMeters = stats.Meters;
        CurrentHoney = stats.Honey;
        CurrentFlock = stats.Flock;
        CurrentBossesKilled = stats.BossesKilled;
        CurrentEnemiesKilled = stats.EnemiesKilled;
        CurrentBombsUsed = stats.BombsUsed;
        CurrentInvulnerability = stats.Invulnerability;

        TotalMeters += CurrentMeters;
        TotalHoney += CurrentHoney;
        TotalFlock += CurrentFlock;
        TotalBossesKilled += CurrentBossesKilled;
        TotalEnemiesKilled += CurrentEnemiesKilled;
        TotalBombsUsed += CurrentBombsUsed;
        TotalInvulnerability += CurrentInvulnerability;
    }
}

public struct Statistics
{
    public int Meters;
    public int Honey;
    public int Flock;
    public int BossesKilled;
    public int EnemiesKilled;
    public int BombsUsed;
    public int Invulnerability;
    public Statistics(int currentMeters, int currentHoney, int currentFlock, int currentBossesKilled, int currentEnemiesKilled, int currentBombsUsed, int currentInvulnerability)
    {
        Meters = currentMeters;
        Honey = currentHoney;
        Flock = currentFlock;
        BossesKilled = currentBossesKilled;
        EnemiesKilled = currentEnemiesKilled;
        BombsUsed = currentBombsUsed;
        Invulnerability = currentInvulnerability;
    }
}
