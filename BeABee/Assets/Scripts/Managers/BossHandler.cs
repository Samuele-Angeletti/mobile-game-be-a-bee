using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        _gameManager.onGameStart += () => _index = 0;
        _gameManager.onGameStart += () => _currentCondition = bossConditionList.First();
        _gameManager.onGameStart += () => Invoke(nameof(PublishCondition), 0.1f);
        _gameManager.onGameOver += () => _spawningBoss = false;

        bossConditionList.ForEach(x => x.CountToDestroyBoss = x.BossPrefab.CountToDestroy);
    }

    private void PublishCondition()
     {
        Publisher.Publish(new BossConditionChangedMessage(_currentCondition));
    }

    public void OnPublish(IMessage message)
    {
        if(message is EnemyKilledMessage enemyKilled)
        {
            if(enemyKilled.EnemyType == EEnemyType.Boss)
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
        if(_gameManager.IsGamePlaying && !_spawningBoss)
        {
            _timePassed += Time.deltaTime;
            if(_timePassed >= timeTryCondition)
            {
                _timePassed = 0;
                if(ConditionMet())
                {
                    _spawningBoss = true;
                    _spawnerManager.Spawn(_currentCondition.BossPrefab);
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
            return bossConditionList.First();
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
        return bossConditionList[_index];
    }
}

[Serializable]
public class BossCondition
{
    public BossSpawnable BossPrefab;
    public float Meters;
    public int Score;
    public int MaxFlockHad;
    public int CountToDestroyBoss;

    public void SetBossCountToDestroy()
    {
        CountToDestroyBoss = Mathf.Clamp(CountToDestroyBoss, CountToDestroyBoss, MaxFlockHad);
        BossPrefab.SetCountToDestroy(CountToDestroyBoss);
    }
}
