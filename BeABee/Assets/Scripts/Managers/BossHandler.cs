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

    BossCondition currentCondition;
    SpawnerManager _spawnerManager;
    float _timePassed;
    bool _spawningBoss;
    int _index;

    private void Awake()
    {
        _spawnerManager = GetComponent<SpawnerManager>();
    }

    private void Start()
    {
        Publisher.Subscribe(this, typeof(EnemyKilledMessage));

        currentCondition = NextCondition();
    }

    public void OnPublish(IMessage message)
    {
        if(message is EnemyKilledMessage enemyKilled)
        {
            if(enemyKilled.EnemyType == EEnemyType.Boss)
            {
                _spawningBoss = false;

                currentCondition = NextCondition();

                if (currentCondition == null)
                    GameManager.Instance.GameOver();
            }
        }
    }

    private void Update()
    {
        if(GameManager.Instance.IsGamePlaying && !_spawningBoss)
        {
            _timePassed += Time.deltaTime;
            if(_timePassed >= timeTryCondition)
            {
                _timePassed = 0;
                if(ConditionMet())
                {
                    _spawningBoss = true;
                    _spawnerManager.Spawn(currentCondition.BossPrefab);
                }
            }
        }
    }

    private bool ConditionMet()
    {
        return GameManager.Instance.MetersDone >= currentCondition.Meters
            && GameManager.Instance.ScoreDone >= currentCondition.Score
            && GameManager.Instance.FlockReached >= currentCondition.MaxFlockHad;
    }

    private BossCondition NextCondition()
    {
        if (currentCondition == null)
        {
            _index = 0;
            return bossConditionList.First();
        }

        if (_index + 1 >= bossConditionList.Count)
            return null;

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
}
