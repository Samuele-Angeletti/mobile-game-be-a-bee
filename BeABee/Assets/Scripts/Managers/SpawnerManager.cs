using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform spawnPosition;
    [SerializeField] Transform deathPosition;
    [SerializeField, Range(0, 8)] float spawnYRange = 0;
    [SerializeField] List<Spawnable> spawnablePrefabList;

    [Header("Settings")]
    [SerializeField] float timeSpawn;
    [SerializeField, Range(1, 100)] float chanceSpawnPickable;
    [SerializeField, Range(1, 100)] float chanceSpawnObstacle;
    [SerializeField, Range(1, 100)] float chanceSpawnEnemy;

    float _timePassed;
    float _chanceSpawnRange => chanceSpawnEnemy + chanceSpawnPickable + chanceSpawnObstacle;

    List<Spawnable> _activeSpawnableList;
    List<Spawnable> _notActiveSpawnableList;
    private void Awake()
    {
        _activeSpawnableList = new List<Spawnable>();
        _notActiveSpawnableList = new List<Spawnable>();
    }

    private void Start()
    {
        GameManager.Instance.onGameOver += () => _activeSpawnableList.ForEach(x => x.Kill());
    }

    private void Update()
    {
        if(GameManager.Instance.IsGameStarted)
        {
            _timePassed += Time.deltaTime;
            if(_timePassed >= timeSpawn)
            {
                _timePassed = 0;
                Spawn();
            }
        }
    }

    private void Spawn()
    {
        var spawnableType = GetSpawnType();
        var selectedSpawnableList = spawnablePrefabList.Where(x => x.SpawnableType == spawnableType).ToList();

        var selectedSpawnablePrefab = selectedSpawnableList[UnityEngine.Random.Range(0, selectedSpawnableList.Count)];

        // TODO: POOL FROM QUEUE - INHERIT EACH OBJECT FROM SPAWNABLE AND CHECK THERE THE TYPE AND THEN THE OBJECTTYPE

        Vector3 spawnPos;

        if (selectedSpawnablePrefab.SpawnableType == ESpawnableTypes.Obstacle)
        {
            spawnPos = spawnPosition.position;
            ObstacleSpawnable obstacleSpawnable = (ObstacleSpawnable)selectedSpawnablePrefab;
            if(_notActiveSpawnableList.Any(x => x.ObstacleType == obstacleSpawnable.ObstacleType))
            {
                PoolSpawnable(_notActiveSpawnableList.Find(x => x.ObstacleType == obstacleSpawnable.ObstacleType), spawnPos);
                return;
            }
        }
        else
        {
            spawnPos = GetRandomSpawnPosition();

            if(selectedSpawnablePrefab.SpawnableType == ESpawnableTypes.Enemy)
            {
                EnemySpawnable enemySpawnable = (EnemySpawnable)selectedSpawnablePrefab;
                if(_notActiveSpawnableList.Any(x => x.EnemyType == enemySpawnable.EnemyType))
                {
                    PoolSpawnable(_notActiveSpawnableList.Find(x => x.EnemyType == enemySpawnable.EnemyType), spawnPos);
                    return;
                }
            }
            else if(selectedSpawnablePrefab.SpawnableType == ESpawnableTypes.Pickable)
            {
                PickableSpawnable pickableSpawnable = (PickableSpawnable)selectedSpawnablePrefab;
                if(_notActiveSpawnableList.Any(x => x.PickableType == pickableSpawnable.PickableType))
                {
                    PoolSpawnable(_notActiveSpawnableList.Find(x => x.PickableType == pickableSpawnable.PickableType), spawnPos);
                    return;
                }
            }
        }
            

        Spawnable newSpawnable = Instantiate(selectedSpawnablePrefab, spawnPos, Quaternion.identity);
        newSpawnable.Initialize(deathPosition.position);

        newSpawnable.onDestroySpawnable += () => _notActiveSpawnableList.Add(newSpawnable);
        newSpawnable.onDestroySpawnable += () => _activeSpawnableList.Remove(newSpawnable);

        _activeSpawnableList.Add(newSpawnable);
    }

    private void PoolSpawnable(Spawnable selectedSpawnable, Vector3 spawnPos)
    {
        _notActiveSpawnableList.Remove(selectedSpawnable);
        selectedSpawnable.transform.position = spawnPos;
        selectedSpawnable.gameObject.SetActive(true);
        _activeSpawnableList.Add(selectedSpawnable);
    }

    private Vector3 GetRandomSpawnPosition()
    {
        return spawnPosition.position + new Vector3(0, UnityEngine.Random.Range(-spawnYRange, spawnYRange), 0);
    }

    private ESpawnableTypes GetSpawnType()
    {
        float randomRange = UnityEngine.Random.Range(1, _chanceSpawnRange);
        if(randomRange <= chanceSpawnPickable)
        {
            return ESpawnableTypes.Pickable;
        }

        if(randomRange <= chanceSpawnPickable + chanceSpawnEnemy)
        {
            return ESpawnableTypes.Enemy;
        }

        return ESpawnableTypes.Obstacle;
    }

#if UNITY_EDITOR
    [Header("Gizmo Settings")]
    [SerializeField] bool activeGizmo = true;
    [SerializeField] float radiusSphere;
    [SerializeField] Color color;
    private void OnDrawGizmos()
    {
        if(activeGizmo)
        {
            if (deathPosition != null && spawnPosition != null)
            {
                Gizmos.color = color;
                Gizmos.DrawSphere(deathPosition.position, radiusSphere);
                Gizmos.DrawSphere(spawnPosition.position, radiusSphere);
                Gizmos.DrawLine(spawnPosition.position, deathPosition.position);
                Gizmos.DrawLine(spawnPosition.position + new Vector3(0, 0.1f, 0), deathPosition.position + new Vector3(0, 0.1f, 0));
                Gizmos.DrawLine(spawnPosition.position - new Vector3(0, 0.1f, 0), deathPosition.position - new Vector3(0, 0.1f, 0));

                Gizmos.DrawSphere(spawnPosition.position + new Vector3(0, spawnYRange, 0), radiusSphere);
                Gizmos.DrawSphere(spawnPosition.position - new Vector3(0, spawnYRange, 0), radiusSphere);
                Gizmos.DrawLine(spawnPosition.position + new Vector3(0, spawnYRange, 0), spawnPosition.position - new Vector3(0, spawnYRange, 0));
            }
        }
        
    }
#endif
}
