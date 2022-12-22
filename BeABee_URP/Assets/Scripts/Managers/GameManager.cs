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
    [SerializeField] float speedDecreaser = 0.01f;

    [HideInInspector] public bool IsGamePlaying;
    [HideInInspector] public float MetersDone = 0;
    [HideInInspector] public int ScoreDone = 0;
    [HideInInspector] public int FlockMax = 0;
    [HideInInspector] public int CurrentFlock = 0;
    [HideInInspector] public int BossesKilled = 0;
    [HideInInspector] public int EnemiesKilled = 0;
    [HideInInspector] public int BombUsed = 0;
    [HideInInspector] public int InvulnerabilityPicked = 0;
    [HideInInspector] public EScenario CurrentScenario => _backgroundManager.ActiveScenery;

    public delegate void OnGameStateChange();
    public OnGameStateChange onGameOver;
    public OnGameStateChange onGameStart;

    InputSystem _inputSystem;
    FlockManager _flockManager;
    UIManager _uiManager;
    BackgroundManager _backgroundManager;
    AudioSource _audioSource;
    private float _meterStep;
    private float _lastTimeScale;
    private float _currentSpeedIncreaser;
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        _inputSystem = new InputSystem();

        _inputSystem.Player.Enable();
        _inputSystem.Player.TouchScreen.performed += JumpPerformed;
        _inputSystem.Player.JumpDEMO.performed += JumpPerformed;

        _uiManager = FindObjectOfType<UIManager>();
        _flockManager = FindObjectOfType<FlockManager>();
        _backgroundManager = FindObjectOfType<BackgroundManager>();

    }
    private void Start()
    {
        _flockManager.UpdateSprite(CurrentScenario);

        Publisher.Subscribe(this, typeof(EnemyKilledMessage));
        Publisher.Subscribe(this, typeof(ChoosingNextScenarioMessage));
        Publisher.Subscribe(this, typeof(ScenarioChoosedMessage));
    }
    private void JumpPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        _flockManager.Jump();

        if(IsGamePlaying)
        {
            _audioSource.Stop();
            _audioSource.Play();
        }
    }

    public void StartGame()
    {
        _flockManager.Initialize();
        _currentSpeedIncreaser = speedIncreaser;
        IsGamePlaying = true;
        onGameStart?.Invoke();
    }

    public void GameOver()
    {
        IsGamePlaying = false;

        PlayerStatistics.SetStatistics(new Statistics((int)MetersDone, ScoreDone, FlockMax, BossesKilled, EnemiesKilled, BombUsed, InvulnerabilityPicked));

        onGameOver?.Invoke();

        Time.timeScale = 1;
        MetersDone = 0;
        FlockMax = 0;
        ScoreDone = 0;
        CurrentFlock = 0;

        _uiManager.ShowFinalStats();
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
        Time.timeScale += _currentSpeedIncreaser;
        _currentSpeedIncreaser -= speedDecreaser;
        _currentSpeedIncreaser = Mathf.Clamp(_currentSpeedIncreaser, speedDecreaser, speedIncreaser);
    }

    public void OnPublish(IMessage message)
    {
        if(message is EnemyKilledMessage enemyKilledMsg)
        {
            ScoreDone += enemyKilledMsg.EnemySpawnable.HoneyOnDestroy;
            switch (enemyKilledMsg.EnemyType)
            {
                case EEnemyType.Boss:
                    BossesKilled++;
                    IncreaseGameSpeed();
                    break;
                default:
                    EnemiesKilled++;
                    break;
            }
        }
        else if(message is ChoosingNextScenarioMessage)
        {
            _lastTimeScale = Time.timeScale;
            Time.timeScale = 1;
            IsGamePlaying = false;
        }
        else if (message is ScenarioChoosedMessage scenario)
        {
            _backgroundManager.ChangeScenery(scenario.CurrentScenario, scenario.GoingUp);
            Time.timeScale = _lastTimeScale;
            IsGamePlaying = true;
        }
    }
    private void OnDestroy()
    {
        Publisher.Unsubscribe(this, typeof(EnemyKilledMessage));
        Publisher.Unsubscribe(this, typeof(ChoosingNextScenarioMessage));
        Publisher.Unsubscribe(this, typeof(ScenarioChoosedMessage));
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
