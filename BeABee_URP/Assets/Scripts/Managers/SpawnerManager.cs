using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnerManager : MonoBehaviour, ISubscriber, ISoundMaker
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

    [Header("Sounds")]
    [SerializeField] AudioClip normalBackground;
    [SerializeField] AudioClip bossBackground;

    float _currentEasyToRandomicStep;
    float _singleDifficultStep;
    float _timePassed;
    float _chanceSpawnRange => chanceSpawnEnemy + chanceSpawnPickable + chanceSpawnObstacle;

    public AudioSource AudioSource { get; set; }
    public string MixerFatherName { get; set; }

    bool _spawningBoss;
    List<Spawnable> _onGameSpawnableList;
    GameManager _gameManager;
    private void Awake()
    {
        _onGameSpawnableList = new List<Spawnable>();
        _gameManager = GameManager.Instance;
        AudioSource = GetComponent<AudioSource>();
        MixerFatherName = SoundManager.Instance.GetMixerFatherName(AudioSource.outputAudioMixerGroup.name);
    }

    private void Start()
    {
        GameManager.Instance.onGameOver += () => _onGameSpawnableList.Where(x => x.gameObject.activeSelf).ToList().ForEach(x => x.Kill());
        GameManager.Instance.onGameOver += () => _spawningBoss = false;
        GameManager.Instance.onGameOver += () => _currentEasyToRandomicStep = _singleDifficultStep - 1;
        GameManager.Instance.onGameStart += () => PlaySound();
        GameManager.Instance.onGameOver += () => StopSound();
        AudioSource.clip = normalBackground;

        _singleDifficultStep = 100 / spawnablePrefabList.Where(x => x.SpawnableType == ESpawnableTypes.Enemy).Select(x => (EnemySpawnable)x).OrderBy(x => x.GetCountToDestroy()).ToList().Count;
        IncreaseEnemySpawnDifficult();
        _currentEasyToRandomicStep -= 1;

        Publisher.Subscribe(this, typeof(SpawnObjectMessage));
        Publisher.Subscribe(this, typeof(EnemyKilledMessage));
        Publisher.Subscribe(this, typeof(BossConditionMetMessage));
    }

    private void Update()
    {
        if(_gameManager.IsGamePlaying && !_spawningBoss)
        {
            _timePassed += Time.deltaTime;
            if(_timePassed >= timeSpawn)
            {
                _timePassed = 0;
                Spawn(null);
            }
        }
    }

    public void Spawn(Spawnable spawnable)
    {
        Spawnable selectedSpawnablePrefab;

        if (spawnable == null)
        {
            var spawnableType = GetSpawnType();
            var selectedSpawnableTypeList = spawnablePrefabList.Where(x => x.SpawnableType == spawnableType).ToList();

            selectedSpawnablePrefab = spawnableType == ESpawnableTypes.Enemy ? GetCircumstancesEenemy(selectedSpawnableTypeList) : selectedSpawnableTypeList[UnityEngine.Random.Range(0, selectedSpawnableTypeList.Count)];
        }
        else
            selectedSpawnablePrefab = spawnable;

        Vector3 spawnPos = spawnPosition.position;
        List<Spawnable> notActiveSpawnableList = null;

        switch (selectedSpawnablePrefab.SpawnableType)
        {
            case ESpawnableTypes.Pickable:
                spawnPos = GetRandomSpawnPosition();
                notActiveSpawnableList = _onGameSpawnableList.Where(x => !x.gameObject.activeSelf && x.SpawnableType == ESpawnableTypes.Pickable).ToList();
                if (notActiveSpawnableList.Any(x => x.PickableType == selectedSpawnablePrefab.PickableType))
                {
                    PoolSpawnable(notActiveSpawnableList.Find(x => x.PickableType == selectedSpawnablePrefab.PickableType), spawnPos);
                    return;
                }
                break;
            case ESpawnableTypes.Enemy:
                spawnPos = GetRandomSpawnPosition();
                notActiveSpawnableList = _onGameSpawnableList.Where(x => !x.gameObject.activeSelf && x.SpawnableType == ESpawnableTypes.Enemy).ToList();
                if (notActiveSpawnableList.Any(x => x.EnemyType == selectedSpawnablePrefab.EnemyType))
                {
                    PoolSpawnable(notActiveSpawnableList.Find(x => x.EnemyType == selectedSpawnablePrefab.EnemyType), spawnPos);
                    return;
                }
                break;
            case ESpawnableTypes.Obstacle:

                notActiveSpawnableList = _onGameSpawnableList.Where(x => !x.gameObject.activeSelf && x.SpawnableType == ESpawnableTypes.Obstacle).ToList();

                if (notActiveSpawnableList.Any(x => x.ObstacleType == selectedSpawnablePrefab.ObstacleType))
                {
                    PoolSpawnable(notActiveSpawnableList.Find(x => x.ObstacleType == selectedSpawnablePrefab.ObstacleType), spawnPos);
                    return;
                }
                break;
            case ESpawnableTypes.Boss:
                _spawningBoss = true;
                StopSound();
                AudioSource.clip = bossBackground;
                PlaySound();
                break;
        }

        Spawnable newSpawnable = Instantiate(selectedSpawnablePrefab, spawnPos, Quaternion.identity);
        newSpawnable.Initialize(deathPosition.position);

        _onGameSpawnableList.Add(newSpawnable);
    }

    private Spawnable GetCircumstancesEenemy(List<Spawnable> selectedSpawnableTypeList)
    {
        var enemyPrefabList = selectedSpawnableTypeList.Select(x => (EnemySpawnable)x).OrderBy(x => x.GetCountToDestroy()).ToList();

        for (int i = 0; i < enemyPrefabList.Count; i++)
        {
            if(_currentEasyToRandomicStep <= _singleDifficultStep * i)
            {
                return i == 0 ? enemyPrefabList[0] : enemyPrefabList[UnityEngine.Random.Range(0, i)];
            }
        }

        return enemyPrefabList[UnityEngine.Random.Range(0, enemyPrefabList.Count)];
    }

    private void PoolSpawnable(Spawnable selectedSpawnable, Vector3 spawnPos)
    {
        selectedSpawnable.gameObject.SetActive(true);
        selectedSpawnable.transform.position = spawnPos;
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

    public void IncreaseEnemySpawnDifficult()
    {
        if (_currentEasyToRandomicStep >= 100)
            return;

        _currentEasyToRandomicStep += _singleDifficultStep;
        if (_currentEasyToRandomicStep >= 100)
            _currentEasyToRandomicStep = 100;
    }

    public void DecreaseEnemySpawnDifficult()
    {
        if (_currentEasyToRandomicStep <= _singleDifficultStep)
            return;

        _currentEasyToRandomicStep -= _singleDifficultStep;
        if (_currentEasyToRandomicStep < 0)
            _currentEasyToRandomicStep = 1;
    }

    public void OnPublish(IMessage message)
    {
        if (message is SpawnObjectMessage spawnMessage)
        {
            Spawn(spawnMessage.ObjectToSpawn);
        }
        else if(message is EnemyKilledMessage enemyKilledMessage)
        {
            if(enemyKilledMessage.EnemyType == EEnemyType.Boss)
            {
                _spawningBoss = false;
                StopSound();
                AudioSource.clip = normalBackground;
                PlaySound();
                IncreaseEnemySpawnDifficult();
            }
        }
    }


    public void PlaySound()
    {
        if (!SoundManager.IsMuted && !SoundManager.Instance.IsMixerMuted(MixerFatherName))
            AudioSource.Play();
    }

    public void StopSound()
    {
        AudioSource.Stop();
    }
    private void OnDisable()
    {
        Publisher.Unsubscribe(this, typeof(SpawnObjectMessage));
        Publisher.Unsubscribe(this, typeof(EnemyKilledMessage));
        Publisher.Unsubscribe(this, typeof(BossConditionMetMessage));
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
