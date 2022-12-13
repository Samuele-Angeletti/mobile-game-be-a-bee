using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour, ISubscriber
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

    [Header("Game Settings")]
    [SerializeField] int increaseSpeedAfterMeters = 200;
    [SerializeField] float speedIncreaser = 0.1f;

    [HideInInspector] public bool IsGamePlaying;
    [HideInInspector] public float MetersDone = 0;
    [HideInInspector] public int ScoreDone = 0;
    [HideInInspector] public int FlockMax = 0;
    [HideInInspector] public int CurrentFlock = 0;

    public delegate void OnGameStateChange();
    public OnGameStateChange onGameOver;
    public OnGameStateChange onGameStart;

    InputSystem _inputSystem;
    FlockManager _flockManager;
    UIManager _uiManager;
    private float _meterStep;
    private float _lastTimeScale;
    private void Awake()
    {
        _inputSystem = new InputSystem();

        _inputSystem.Player.Enable();
        _inputSystem.Player.TouchScreen.performed += JumpPerformed;
        _inputSystem.Player.JumpDEMO.performed += JumpPerformed;

        _uiManager = FindObjectOfType<UIManager>();
        _flockManager = FindObjectOfType<FlockManager>();
    }
    private void Start()
    {
        Publisher.Subscribe(this, typeof(EnemyKilledMessage));
    }
    private void JumpPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        _flockManager.Jump();
    }

    public void StartGame()
    {
        _flockManager.Initialize();
        IsGamePlaying = true;
        onGameStart?.Invoke();
    }

    public void GameOver()
    {
        IsGamePlaying = false;

        onGameOver?.Invoke();

        Time.timeScale = 1;
        MetersDone = 0;
        FlockMax = 0;
        ScoreDone = 0;
        CurrentFlock = 0;

        _uiManager.ResetMenu();
    }

    private void Update()
    {
        if(IsGamePlaying)
        {
            MetersDone += Time.deltaTime;

            _meterStep += Time.deltaTime;
            if(_meterStep >= increaseSpeedAfterMeters)
            {
                _meterStep = 0;
                IncreaseGameSpeed();
            }

            CurrentFlock = _flockManager.ActiveBeeCount;
        }
    }

    public void IncreaseGameSpeed()
    {
        Time.timeScale += speedIncreaser;
    }

    public void OnPublish(IMessage message)
    {
        if(message is EnemyKilledMessage enemyKilledMsg)
        {
            switch (enemyKilledMsg.EnemyType)
            {
                case EEnemyType.TwoBees:
                    ScoreDone += 10;
                    break;
                case EEnemyType.ThreeBees:
                    ScoreDone += 20;
                    break;
                case EEnemyType.FourBees:
                    ScoreDone += 30;
                    break;
                case EEnemyType.FiveBees:
                    ScoreDone += 40;
                    break;
                case EEnemyType.SixBees:
                    ScoreDone += 50;
                    break;
                case EEnemyType.SevenBees:
                    ScoreDone += 60;
                    break;
                case EEnemyType.Boss:
                    ScoreDone += 100;
                    IncreaseGameSpeed();
                    break;
            }
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void PauseGame()
    {
        if(Time.timeScale == 0)
        {
            ResumeGame();
            return;
        }

        _lastTimeScale = Time.timeScale;
        Time.timeScale = 0;
        IsGamePlaying = false;
    }

    private void ResumeGame()
    {
        Time.timeScale = _lastTimeScale;
        IsGamePlaying = true;
    }
}
