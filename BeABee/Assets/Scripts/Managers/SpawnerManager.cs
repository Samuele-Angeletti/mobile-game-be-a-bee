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
    [SerializeField] List<Spawnable> spawnablePrefabList;

    [Header("Settings")]
    [SerializeField] float timeSpawn;
    [SerializeField, Range(1, 100)] float chanceSpawnPickable;
    [SerializeField, Range(1, 100)] float chanceSpawnObstacle;
    [SerializeField, Range(1, 100)] float chanceSpawnEnemy;


    float _timePassed;
    float chanceSpawnRange => chanceSpawnEnemy + chanceSpawnPickable + chanceSpawnObstacle;
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
        var selectedSpawnableList = spawnablePrefabList.Where(x => x.SpawnableTypes == spawnableType).ToList();

        var selectedSpawnable = selectedSpawnableList[UnityEngine.Random.Range(0, selectedSpawnableList.Count)];

        // TODO: POOL FROM QUEUE - INHERIT EACH OBJECT FROM SPAWNABLE AND CHECK THERE THE TYPE AND THEN THE OBJECTTYPE

        var newSpawnable = Instantiate(selectedSpawnable, spawnPosition.position, Quaternion.identity);
        newSpawnable.Initialize(deathPosition.position);
    }

    private ESpawnableTypes GetSpawnType()
    {
        float randomRange = UnityEngine.Random.Range(1, chanceSpawnRange);
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
    private void OnDrawGizmos()
    {
        if(deathPosition != null && spawnPosition != null)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(deathPosition.position, 0.3f);
            Gizmos.DrawSphere(spawnPosition.position, 0.2f);
            Gizmos.DrawLine(spawnPosition.position, deathPosition.position);
            Gizmos.DrawLine(spawnPosition.position + new Vector3(0, 0.1f, 0), deathPosition.position + new Vector3(0, 0.1f, 0));
            Gizmos.DrawLine(spawnPosition.position - new Vector3(0, 0.1f, 0), deathPosition.position - new Vector3(0, 0.1f, 0));
        }
    }
#endif
}
