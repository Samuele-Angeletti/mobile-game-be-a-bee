using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FlockManager : MonoBehaviour, ISubscriber
{
    [SerializeField] Bee beePrefab;
    [SerializeField] Sprite spaceSprite;
    [SerializeField] Sprite skySprite;
    [SerializeField] Sprite mountainSprite;
    [SerializeField] Sprite forestSprite;
    [SerializeField] Sprite undergroundSprite;

    public Transform FrontFlockPosition;
    public Transform MinPosition;
    public Transform MaxPosition;
    public float YRandomPositionOnJump;

    public int ActiveBeeCount => _beeList.Where(x => x.gameObject.activeSelf).Count();

    List<Bee> _beeList;
    Bee _leaderBee;

    Queue<Bee> _beeQueue;
    private void Awake()
    {
        _beeList = new List<Bee>();
        _beeQueue = new Queue<Bee>();
    }

    private void Start()
    {
        FrontFlockPosition.transform.parent = null;
        MinPosition.transform.parent = null;
        MaxPosition.transform.parent = null;

        Publisher.Subscribe(this, typeof(EnemyKilledMessage));
    }

    public void Initialize()
    {
        SetNewLeader(SpawnBee(FrontFlockPosition.position, true, mountainSprite));
    }

    private Bee SpawnBee(Vector3 position, bool isLeader, Sprite sprite)
    {
        Bee newBee = _beeQueue.Count > 0 ? _beeQueue.Dequeue() : Instantiate(beePrefab, position, Quaternion.identity);

        if (!newBee.gameObject.activeSelf)
        {
            newBee.gameObject.SetActive(true);
            newBee.transform.position = position;
        }
        else
        {
            newBee.onKilled += () => _beeQueue.Enqueue(newBee);
            newBee.onKilled += () => _beeList.Remove(newBee);
        }

        newBee.Initialize(isLeader, sprite, this);
        _beeList.Add(newBee);
        if(_leaderBee != null)
            _beeList.ForEach(x => x.SetFlockLeader(_leaderBee));
        if (!newBee.IsLeader)
            newBee.SetXDestination(newBee.transform.position);

        return newBee;
    }

    public void SpawnRandomBee()
    {
        SpawnBee(transform.position + GetRandomXPosition(), false, mountainSprite);
    }

    private void Update()
    {
        if (_leaderBee != null)
            transform.position = _leaderBee.transform.position;
        else
            transform.position = Vector3.zero;
    }

    private Vector3 GetRandomXPosition()
    {
        return new Vector3(Random.Range(MinPosition.position.x, MaxPosition.position.x), Random.Range(MinPosition.position.y, MaxPosition.position.y), 0);
    }

    public void Jump()
    {
        foreach (var bee in _beeList)
        {
            bee.SetRandomY(-YRandomPositionOnJump, YRandomPositionOnJump);
            bee.Jump();
        }
    }

    public void SetNewLeader(Bee bee = null)
    {
        _leaderBee = bee != null 
            ? bee 
            : _beeList.Count > 0 
            ? _beeList.First()
            : null;

        if(_leaderBee == null)
        {
            GameManager.Instance.GameOver();
            return;
        }

        _leaderBee.SetLeader();
        _beeList.ForEach(x => x.SetFlockLeader(_leaderBee));
    }

    public void OnPublish(IMessage message)
    {
        if(message is EnemyKilledMessage)
        {
            _leaderBee.SetXDestination(FrontFlockPosition.position);
            foreach (Bee bee in _beeList.Where(x => !x.IsLeader))
            {
                bee.SetXDestination(GetRandomXPosition());
            }
        }
    }

    private void OnDisable()
    {
        Publisher.Unsubscribe(this, typeof(EnemyKilledMessage));
    }

#if UNITY_EDITOR
    [Header("Gizmo Settings")]
    [SerializeField] bool activeGizmo = true;
    private void OnDrawGizmos()
    {
        if(activeGizmo)
        {
            if (FrontFlockPosition != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(FrontFlockPosition.position, 0.3f);

                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(FrontFlockPosition.position + new Vector3(-2, YRandomPositionOnJump), 0.2f);
                Gizmos.DrawWireSphere(FrontFlockPosition.position - new Vector3(2, YRandomPositionOnJump), 0.2f);
                Gizmos.DrawLine(FrontFlockPosition.position + new Vector3(-2, YRandomPositionOnJump), FrontFlockPosition.position - new Vector3(2, YRandomPositionOnJump));
            }

            if (MinPosition != null && MaxPosition != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(MinPosition.position, 0.3f);
                Gizmos.DrawSphere(MaxPosition.position, 0.3f);

                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(MinPosition.position, new Vector3(MinPosition.position.x, MaxPosition.position.y, 0));
                Gizmos.DrawLine(MinPosition.position, new Vector3(MaxPosition.position.x, MinPosition.position.y, 0));
                Gizmos.DrawLine(MaxPosition.position, new Vector3(MinPosition.position.x, MaxPosition.position.y, 0));
                Gizmos.DrawLine(MaxPosition.position, new Vector3(MaxPosition.position.x, MinPosition.position.y, 0));
            }
        }
        
    }
#endif
}
