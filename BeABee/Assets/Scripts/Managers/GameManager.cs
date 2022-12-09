using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PubSub;
using System;

public class GameManager : MonoBehaviour, ISubscriber
{
    [Header("Game Settings")]
    [SerializeField] List<BirdVariant> m_BirdVariants;

    [Tooltip("The meters done in one second of Time.deltaTime on Game Playing")]
    [SerializeField] float m_MetersOnSecond;

    [Tooltip("The meters to pass before the GameSpeed increase")]
    [SerializeField] List<int> m_MetersStep;

    [Tooltip("This value is multiplied to the GameTime")]
    [SerializeField] float m_SpeedGameIncreaser;

    [Header("Spawn Boss Settings")]
    [SerializeField] List<SpawnBossCondition> m_BossConditions;

    [Header("Scene References")]
    [SerializeField] public UIManager UIManager;
    [SerializeField] FlockHandler m_FlockHandler;
    [SerializeField] SpawnerManager m_SpawnerManager;
    [SerializeField] WorldConfineHandler m_WorldConfineHandler;

    private int m_BirdsQuantity;
    private float m_GameTimeScale;
    private ScoreManager m_ScoreManager;
    private int m_MetersIndex;
    private SpawnBossCondition m_CurrentCondition;
    private int m_BossConditionIndexer = 0;

    private Coroutine m_CheckBossConditionsCoroutine;

    private static GameManager m_Instance;
    public static GameManager Instance
    {
        get => m_Instance;
    }

    public InputSystem Inputs;

    private void Awake()
    {
        Inputs = new InputSystem();

        m_Instance = this;

        Inputs.Enable();
        Inputs.Player.Enable();
        m_GameTimeScale = Time.timeScale;

        m_BossConditions.ForEach(x => x.Initialize());

    }

    void Start()
    {
        PubSub.PubSub.Subscribe(this, typeof(PauseGameMessage));
        PubSub.PubSub.Subscribe(this, typeof(ResumeGameMessage));
        m_ScoreManager = new ScoreManager(m_MetersOnSecond, UIManager);

        StartNextBossCondition();
    }

    void Update()
    {
        m_ScoreManager.Update();
        MetersCheck();

    }

    private IEnumerator CheckBossCondition()
    {
        while (true)
        {
            Debug.Log("Check condition");
            yield return new WaitForSeconds(2f);
            if (m_CurrentCondition.CheckConditionMet(m_ScoreManager.Score, m_ScoreManager.Meters, m_BirdsQuantity, m_ScoreManager.PickablePicked, m_ScoreManager.ObstacleDestroyed))
            {
                m_SpawnerManager.SpawnBoss(m_CurrentCondition.Boss);
                m_WorldConfineHandler.Confine();
                StopCoroutine(m_CheckBossConditionsCoroutine);
                m_CheckBossConditionsCoroutine = null;
            }
        }
    }

    private void MetersCheck()
    {
        if (m_MetersIndex < m_MetersStep.Count)
        {
            if (m_ScoreManager.Meters >= m_MetersStep[m_MetersIndex])
            {
                m_MetersIndex++;
                IncreaseGameSpeed();
            }
        }
    }

    private void IncreaseGameSpeed()
    {
        Time.timeScale *= m_SpeedGameIncreaser;
        m_FlockHandler.Increase();
    }

    public BirdVariant GetBirdVariantByEnum(EBirdType birdType)
    {
        return m_BirdVariants.Find(x => x.BirdType == birdType);
    }

    public void OnPublish(IMessage message)
    {
        if (message is PauseGameMessage)
        {
            m_GameTimeScale = Time.timeScale;
            Time.timeScale = 0;
        }
        else if (message is ResumeGameMessage)
        {
            Time.timeScale = m_GameTimeScale;
        }
    }

    public void AddBird()
    {
        m_BirdsQuantity++;
    }

    public void StartNextBossCondition()
    {
        if (m_CheckBossConditionsCoroutine == null)
        {
            if (m_BossConditionIndexer < m_BossConditions.Count)
            {
                m_CurrentCondition = m_BossConditions[m_BossConditionIndexer];
                m_CheckBossConditionsCoroutine = StartCoroutine(CheckBossCondition());
                m_BossConditionIndexer++;
                m_WorldConfineHandler.ReversePointsToLastPositions();
            }
            else
            {
                PubSub.PubSub.Publish(new GameOverMessage());
            }
        }
    }


    public void ActOnEnum(EPivot pivotSwitcher, Action aCenter, Action aUpOne, Action aUpTwo, Action aUpThree, Action aDownOne, Action aDownTwo, Action aDownThree)
    {
        switch (pivotSwitcher)
        {
            case EPivot.Center:
                aCenter.Invoke();
                break;

            case EPivot.UpOne:
                aUpOne.Invoke();
                break;

            case EPivot.UpTwo:
                aUpTwo.Invoke();
                break;

            case EPivot.UpThree:
                aUpThree.Invoke();
                break;

            case EPivot.DownOne:
                aDownOne.Invoke();
                break;

            case EPivot.DownTwo:
                aDownTwo.Invoke();
                break;

            case EPivot.DownThree:
                aDownThree.Invoke();
                break;

        }
    }
}
