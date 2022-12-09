using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PubSub;

public class FlockHandler : MonoBehaviour, ISubscriber
{
    [SerializeField] Bird m_BirdPrefab;

    [Header("Scene References")]
    [SerializeField] Player m_Player;
    [SerializeField] Transform m_PivotFrontFlock;

    [Header("Flock Handler Settings")]
    [Tooltip("The range on X axe where to spawn a new bird. The values should be lass than the PivotFrontFlock X and should be lass than Max")]
    [SerializeField] float m_MinXSpawn = -3f;
    [Tooltip("The range on X axe where to spawn a new bird. The values should be lass than the PivotFrontFlock X and should be more than Min")]
    [SerializeField] float m_MaxXSpawn = -0.5f;
    [Tooltip("Changes the position of every bird at every jump relative to the FlockLeader. Suggest: start with small values such as -0.3f or -0.5f")]
    [SerializeField] float m_MinRandomPos = -0.5f;
    [Tooltip("Changes the position of every bird at every jump relative to the FlockLeader. Suggest: start with small values such as 0.3f or 0.5f")]
    [SerializeField] float m_MaxRandomPos = 0.5f;
    [Tooltip("Multiplies the Min and Max random position based on Time.timeScale. See GameManager for timeScale increaser")]
    [SerializeField] float m_RandomPosIncreaser = 1.01f;
    [SerializeField] float m_InvulnerabilityAllTime = 3f;

    private List<Bird> m_Birds = new List<Bird>();

    private List<Bird> m_KilledBirds = new List<Bird>();

    private BirdVariant m_NextBirdVariant;
    private Bird m_FlockLeader;
    private Coroutine m_ChangeFlockCoroutine;
    private UIManager m_UIManager;

    public Bird FlockLeader => m_FlockLeader;
    public Vector3 FrontFlockPosition => m_PivotFrontFlock.position;


    void Start()
    {
        PubSub.PubSub.Subscribe(this, typeof(AddBirdMessage));
        PubSub.PubSub.Subscribe(this, typeof(DoubleFlockMessage));
        PubSub.PubSub.Subscribe(this, typeof(HalveFlockMessage));
        PubSub.PubSub.Subscribe(this, typeof(InvulnerabilityAllMessage));
        PubSub.PubSub.Subscribe(this, typeof(KillOneRandomBirdMessage));
        PubSub.PubSub.Subscribe(this, typeof(ObstaclePassedMessage));
        m_UIManager = GameManager.Instance.UIManager;
    }

    private void AddNewBird(Bird newBird)
    {
        m_Birds.Add(newBird);

        if (m_Birds.Count == 1)
            SetLeader();
    }

    public void Jump()
    {
        if(m_FlockLeader != null)
            m_FlockLeader.Jump();

        foreach (Bird b in m_Birds)
        {
            if (!b.IsLeader)
            {
                b.SetRandomPos(m_MinRandomPos, m_MaxRandomPos);
            }
        }
    }

    public void OnPublish(IMessage message)
    {
        if (message is AddBirdMessage)
        {
            AddBirdMessage addOneBird = (AddBirdMessage)message;
            m_NextBirdVariant = addOneBird.BirdVariant;

            if (addOneBird.DoubleBirds)
            {
                foreach (Bird b in m_Birds)
                {
                    if (b.BirdType == m_NextBirdVariant.BirdType)
                    {
                        SpawnNewBird();
                    }
                }
            }
            else
            {
                SpawnNewBird();
            }
        }
        else if (message is DoubleFlockMessage)
        {
            if (m_ChangeFlockCoroutine == null)
                m_ChangeFlockCoroutine = StartCoroutine(ChangeFlockCoroutine(SpawnNewBird, false));
        }
        else if (message is HalveFlockMessage)
        {
            if (m_ChangeFlockCoroutine == null)
                m_ChangeFlockCoroutine = StartCoroutine(ChangeFlockCoroutine(KillOneRandomBird, true));
        }
        else if (message is InvulnerabilityAllMessage)
        {
            m_Birds.ForEach(x => x.StartInvulnerability(m_InvulnerabilityAllTime));
        }
        else if (message is KillOneRandomBirdMessage)
        {
            KillOneRandomBird();
        }
        else if (message is ObstaclePassedMessage)
        {
            StartCoroutine(PassedObstacleCoroutine());
        }
    }

    private IEnumerator PassedObstacleCoroutine()
    {
        yield return new WaitForEndOfFrame();
        foreach (Bird b in m_Birds)
        {
            b.OnXDestination = false;
            b.CanMove = true;
            b.gameObject.transform.parent = null;
            if (!b.IsLeader)
            {
                b.XDestination = SetRandomXPosition(b.transform.position.y);
            }
        }
    }

    private IEnumerator ChangeFlockCoroutine(Action action, bool halving)
    {
        int count = halving ? m_Birds.Count / 2 : m_Birds.Count;
        for (int i = 0; i < count; i++)
        {
            action.Invoke();
            yield return new WaitForSeconds(1f);
        }
        m_ChangeFlockCoroutine = null;
    }

    public Vector3 GetLeaderPosition()
    {
        if (m_FlockLeader != null && m_FlockLeader.gameObject.activeSelf)
            return m_FlockLeader.transform.position;
        else
            return Vector3.zero;

    }

    private void SpawnNewBird()
    {
        Bird newBird = m_KilledBirds.Find(x => x.BirdType == m_NextBirdVariant.BirdType);

        if (newBird == null)
        {
            newBird = Instantiate(m_BirdPrefab);
            newBird.OnKillBird += (newBird) => RemoveBirdFromList(newBird);
        }
        else
        {
            m_KilledBirds.Remove(newBird);
        }

        newBird.Initialize(m_NextBirdVariant, this, SetRandomXPosition(m_Player.transform.position.y), Quaternion.identity);
        AddNewBird(newBird);
        GameManager.Instance.AddBird();
        m_UIManager.UpdateFlockAmountDisplay(m_Birds.Count);
    }

    private Vector3 SetRandomXPosition(float y)
    {
        return new Vector3(UnityEngine.Random.Range(m_MinXSpawn, m_MaxXSpawn), y, 0);
    }

    private void KillOneRandomBird()
    {
        int rndNum = UnityEngine.Random.Range(0, m_Birds.Count);
        m_Birds[rndNum].Kill();
    }

    public void RemoveBirdFromList(Bird bird)
    {
        if (bird == m_FlockLeader)
        {
            m_Birds.Remove(bird);
            SetLeader();
        }
        else
            m_Birds.Remove(bird);

        m_KilledBirds.Add(bird);

        m_UIManager.UpdateFlockAmountDisplay(m_Birds.Count);
    }

    private void SetLeader()
    {
        if (m_Birds.Count > 0)
        {
            int rndNum = UnityEngine.Random.Range(0, m_Birds.Count);
            m_Birds[rndNum].SetLeader(true);
            m_FlockLeader = m_Birds[rndNum];
        }
        else
        {
            MessageSender.SendMessage(EMessageType.GameOver);
            Debug.Log("game over");
        }
    }

    public void Increase()
    {
        m_MinRandomPos *= m_RandomPosIncreaser;
        m_MaxRandomPos *= m_RandomPosIncreaser;

        Debug.Log($"min: {m_MinRandomPos}   max: {m_MaxRandomPos}");
    }
}
