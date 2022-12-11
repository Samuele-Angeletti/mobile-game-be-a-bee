using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region SINGLETON
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
                if (instance != null)
                    return instance;

                GameObject go = new GameObject("GameManager");
                return go.AddComponent<GameManager>();
            }
            else
                return instance;
        }
        set
        {
            instance = value;
        }
    }
    #endregion

    [HideInInspector] public bool IsGamePlaying;
    [HideInInspector] public float MetersDone = 0;
    [HideInInspector] public int ScoreDone = 0;
    [HideInInspector] public int FlockReached = 0;

    public delegate void OnGameOver();
    public OnGameOver onGameOver;

    InputSystem inputSystem;
    FlockManager flockManager;
    SpawnerManager spawnerManager;
    UIManager uiManager;


    private void Awake()
    {
        inputSystem = new InputSystem();

        inputSystem.Player.Enable();
        inputSystem.Player.TouchScreen.performed += JumpPerformed;
        inputSystem.Player.JumpDEMO.performed += JumpPerformed;

        uiManager = FindObjectOfType<UIManager>();
        flockManager = FindObjectOfType<FlockManager>();
        spawnerManager = FindObjectOfType<SpawnerManager>();
    }

    private void JumpPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        flockManager.Jump();
    }

    public void StartGame()
    {
        flockManager.Initialize();
        IsGamePlaying = true;
    }

    public void GameOver()
    {
        IsGamePlaying = false;

        onGameOver?.Invoke();

        MetersDone = 0;
        FlockReached = 0;
        ScoreDone = 0;

        uiManager.ResetMenu();
    }

    private void Update()
    {
        if(IsGamePlaying)
        {
            MetersDone += Time.deltaTime;

            FlockReached = flockManager.ActiveBeeCount;
        }
    }
}
