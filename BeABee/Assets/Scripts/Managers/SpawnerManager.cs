using PubSub;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerManager : MonoBehaviour, ISubscriber
{
    [Header("Pickables Settings")]
    [SerializeField] List<ScriptableObject> m_PickableVariantsSO;
    [SerializeField] Pickable m_PickablePrefab;

    [Header("Ostacles Settings")]
    [SerializeField] List<ScriptableObject> m_ObstacleVariantsSO;
    [SerializeField] Obstacle m_ObstaclePrefab;

    [Header("Pivots Settings")]
    [SerializeField] Transform m_SpawnPivotCenter;
    [SerializeField] Transform m_SpawnPivotUpOne;
    [SerializeField] Transform m_SpawnPivotUpTwo;
    [SerializeField] Transform m_SpawnPivotUpThree;
    [SerializeField] Transform m_SpawnPivotDownOne;
    [SerializeField] Transform m_SpawnPivotDownTwo;
    [SerializeField] Transform m_SpawnPivotDownThree;

    [Space(5)]
    [Header("Common Settings")]
    [SerializeField] float m_TimeSpawn;
    [Tooltip("After this limit the object destroys itself. All the objects go from right to left, so this value should be less than 0")]
    [SerializeField] float m_XLimitOffset = -10;
    [Tooltip("From 0 to 1 the chance in percentage to spawn a Pickable. 1 = Pickable; 0 = Obstacle")]
    [SerializeField] float m_ChanceToSpawnPickable;
    [Tooltip("If any Pickable isn't spawned in raw within this value, so it forces to spawn a Pickable ignoring the ChanceToSpawnPickable percentage. To work this value must be more than 0")]
    [SerializeField] int m_ForceSpawnPickable = 5;

    private List<IScriptableObject> m_PickableVariants = new List<IScriptableObject>();
    private List<IScriptableObject> m_ObstacleVariants = new List<IScriptableObject>();

    private List<ISpawnable> m_Pickables = new List<ISpawnable>();
    private List<ISpawnable> m_Obstacles = new List<ISpawnable>();

    private List<ISpawnable> m_SpawnablesRunning = new List<ISpawnable>();

    private bool m_GameOnPlay;
    private bool m_SpawningBoss;
    private float m_TimePassed;
    private int m_SpawnedObstacleInRawCount;
    private Transform m_CurrentPivot;


    private void Start()
    {
        PubSub.PubSub.Subscribe(this, typeof(GameStartMessage));
        PubSub.PubSub.Subscribe(this, typeof(PauseGameMessage));
        PubSub.PubSub.Subscribe(this, typeof(ResumeGameMessage));
        PubSub.PubSub.Subscribe(this, typeof(GameOverMessage));
        PubSub.PubSub.Subscribe(this, typeof(ObstaclePassedMessage));
        PubSub.PubSub.Subscribe(this, typeof(SetWorldMessage));

        m_ChanceToSpawnPickable = Mathf.Clamp01(m_ChanceToSpawnPickable);

        m_ObstacleVariantsSO.ForEach(x => m_ObstacleVariants.Add((IScriptableObject)x));
        m_PickableVariantsSO.ForEach(x => m_PickableVariants.Add((IScriptableObject)x));
        m_CurrentPivot = m_SpawnPivotCenter;
    }

    private void Update()
    {
        if (m_GameOnPlay && !m_SpawningBoss)
        {
            m_TimePassed += Time.deltaTime;
            if (m_TimePassed >= m_TimeSpawn)
            {
                m_TimePassed = 0;

                Spawn();
            }
        }
    }


    private void Spawn()
    {
        float rndNum = Random.Range(0, 1f);

        if (rndNum <= m_ChanceToSpawnPickable || m_SpawnedObstacleInRawCount >= m_ForceSpawnPickable)
        {
            SpawnObject(m_PickableVariants, m_Pickables, m_PickablePrefab);
            m_SpawnedObstacleInRawCount = 0;
        }
        else
        {
            SpawnObject(m_ObstacleVariants, m_Obstacles, m_ObstaclePrefab);
            m_SpawnedObstacleInRawCount++;
        }
    }

    private void SpawnObject(List<IScriptableObject> variantsList, List<ISpawnable> spawnablesList, ISpawnable spawnablePrefab)
    {
        int rndObject = Random.Range(0, variantsList.Count);

        ISpawnable spawnedObject = spawnablesList.Find(x => x.GetVariant() == (ScriptableObject)variantsList[rndObject]);
        float rnd = Random.Range(0f, 0.99999f);
        bool newPickable = false;
        if (spawnedObject == null || spawnedObject.GetGameObject().activeSelf)
        {
            GameObject g = Instantiate(spawnablePrefab.GetGameObject());
            spawnedObject = g.GetComponent<ISpawnable>();
            spawnedObject.OnDeactive += (spawnedObject) => spawnablesList.Add(spawnedObject);
            spawnedObject.OnDeactive += (spawnedObject) => m_SpawnablesRunning.Remove(spawnedObject);
            newPickable = true;
        }
        if(newPickable) spawnedObject.Initialize((ScriptableObject)variantsList[rndObject], m_XLimitOffset, m_CurrentPivot.position, Quaternion.identity);

        if (rnd < spawnedObject.GetProbabilityToSpawn())
        {
            m_SpawnablesRunning.Add(spawnedObject);
            if(!newPickable)
                spawnedObject.Initialize((ScriptableObject)variantsList[rndObject], m_XLimitOffset, m_CurrentPivot.position, Quaternion.identity);
        }
        else
        {
            spawnedObject.DestroyMe(0.01f);
        }
    }

    public void SpawnBoss(ScriptableObject boss)
    {
        Debug.Log("!!!!!! SPAWNING BOSS !!!!!!!");

        m_SpawningBoss = true;

        List<IScriptableObject> bossList = new List<IScriptableObject> { (IScriptableObject)boss };
        SpawnObject(bossList, m_Obstacles, m_ObstaclePrefab);
    }

    public void OnPublish(IMessage message)
    {
        if (message is GameStartMessage || message is ResumeGameMessage)
        {
            m_GameOnPlay = true;
        }
        else if (message is PauseGameMessage)
        {
            m_GameOnPlay = false;
        }
        else if (message is GameOverMessage)
        {
            m_GameOnPlay = false;
            DestroyAllSpawnableOnList(m_Pickables, 1f);
            DestroyAllSpawnableOnList(m_Obstacles, 1f);

            // check this it really works? or the other 2 are already fine?
            DestroyAllSpawnableOnList(m_SpawnablesRunning, 1f);
        }
        else if (message is ObstaclePassedMessage)
        {
            ObstaclePassedMessage obstaclePassed = (ObstaclePassedMessage)message;
            if (obstaclePassed.IsBoss && m_SpawningBoss)
            {
                m_SpawningBoss = false;
                GameManager.Instance.StartNextBossCondition();
            }
        }
        else if (message is SetWorldMessage)
        {
            SetWorldMessage setWorld = (SetWorldMessage)message;
            GameManager.Instance.ActOnEnum(
        setWorld.NewPivot,
        () => m_CurrentPivot = m_SpawnPivotCenter,
        () => m_CurrentPivot = m_SpawnPivotUpOne,
        () => m_CurrentPivot = m_SpawnPivotUpTwo,
        () => m_CurrentPivot = m_SpawnPivotUpThree,
        () => m_CurrentPivot = m_SpawnPivotDownOne,
        () => m_CurrentPivot = m_SpawnPivotDownTwo,
        () => m_CurrentPivot = m_SpawnPivotDownThree);
        }
    }

    private void DestroyAllSpawnableOnList(List<ISpawnable> objectsSpawned, float timeDelay)
    {
        objectsSpawned.ForEach(x => x.DestroyMe(timeDelay));
    }

}
