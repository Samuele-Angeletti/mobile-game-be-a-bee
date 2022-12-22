using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

[RequireComponent(typeof(SpawnerManager))]
public class BossHandler : MonoBehaviour, ISubscriber
{
    [SerializeField] float timeTryCondition = 2;
    [SerializeField] List<BossCondition> bossConditionList;

    BossCondition _currentCondition;
    SpawnerManager _spawnerManager;
    float _timePassed;
    bool _spawningBoss;
    int _index;
    GameManager _gameManager;
    private void Awake()
    {
        _spawnerManager = GetComponent<SpawnerManager>();
    }

    private void Start()
    {
        Publisher.Subscribe(this, typeof(EnemyKilledMessage));
        _gameManager = GameManager.Instance;
        _gameManager.onGameStart += () => Initialize();
        _gameManager.onGameOver += () => _spawningBoss = false;
        _gameManager.onGameOver += () => _currentCondition = null;
    }

    private void Initialize()
    {
        _index = 0;
        _currentCondition = NextCondition();
        Invoke(nameof(PublishCondition), 0.1f);
    }

    private void PublishCondition()
    {
        Publisher.Publish(new BossConditionChangedMessage(_currentCondition));
    }

    public void OnPublish(IMessage message)
    {
        if (message is EnemyKilledMessage enemyKilled)
        {
            if (enemyKilled.EnemyType == EEnemyType.Boss)
            {
                _spawningBoss = false;

                _currentCondition = NextCondition();
                PublishCondition();

                if (_currentCondition == null)
                    _gameManager.GameOver();
            }
        }
    }

    private void Update()
    {
        if (_gameManager.IsGamePlaying && !_spawningBoss)
        {
            _timePassed += Time.deltaTime;
            if (_timePassed >= timeTryCondition)
            {
                _timePassed = 0;
                if (ConditionMet())
                {
                    _spawningBoss = true;
                    _spawnerManager.Spawn(_currentCondition.BossPrefab);
                    Publisher.Publish(new BossConditionMetMessage());
                }
            }
        }
    }

    private bool ConditionMet()
    {
        return _gameManager.MetersDone >= _currentCondition.Meters
            && _gameManager.ScoreDone >= _currentCondition.Score
            && _gameManager.FlockMax >= _currentCondition.MaxFlockHad;
    }

    private BossCondition NextCondition()
    {
        if (_currentCondition == null)
        {
            _index = 0;
            var bossCondition = bossConditionList.First();
            bossCondition.SetBossCountToDestroy();
            return bossCondition;
        }

        if (_index + 1 >= bossConditionList.Count)
        {
            var newCondition = new BossCondition()
            {
                BossPrefab = bossConditionList[UnityEngine.Random.Range(0, bossConditionList.Count)].BossPrefab,
                Meters = _currentCondition.Meters + UnityEngine.Random.Range(10, _currentCondition.Meters),
                Score = _currentCondition.Score + UnityEngine.Random.Range(10, _currentCondition.Score + 1),
                MaxFlockHad = _currentCondition.MaxFlockHad + UnityEngine.Random.Range(10, _currentCondition.MaxFlockHad + 1),
                CountToDestroyBoss = _currentCondition.CountToDestroyBoss + UnityEngine.Random.Range(1, _currentCondition.CountToDestroyBoss + 1)
            };

            newCondition.SetBossCountToDestroy();

            bossConditionList.Add(newCondition);
        }

        _index++;
        var newBossCondition = bossConditionList[_index];
        newBossCondition.SetBossCountToDestroy();
        return newBossCondition;
    }
}

[Serializable]
public class BossCondition
{
    public BossSpawnable BossPrefab;
    public float Meters;
    public int Score;
    public int MaxFlockHad;
    [Range(1,100)] public int CountToDestroyBoss;

    public void SetBossCountToDestroy()
    {
        if (CountToDestroyBoss > MaxFlockHad)
            CountToDestroyBoss = MaxFlockHad;
        BossPrefab.SetCountToDestroy(CountToDestroyBoss);
    }
}
