using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FlockManager : MonoBehaviour
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

    List<Bee> beeList;
    Bee leaderBee;
    float debugTimePass;
    Queue<Bee> beeQueue;
    private void Awake()
    {
        beeList = new List<Bee>();
        beeQueue = new Queue<Bee>();
    }

    private void Start()
    {
        FrontFlockPosition.transform.parent = null;
        MinPosition.transform.parent = null;
        MaxPosition.transform.parent = null;
    }

    public void Initialize()
    {
        SetNewLeader(SpawnBee(FrontFlockPosition.position, true, mountainSprite));
    }

    private Bee SpawnBee(Vector3 position, bool isLeader, Sprite sprite)
    {
        Bee newBee = beeQueue.Count > 0 ? beeQueue.Dequeue() : Instantiate(beePrefab, position, Quaternion.identity);

        if (!newBee.gameObject.activeSelf)
        {
            newBee.gameObject.SetActive(true);
            newBee.transform.position = position;
        }
        else
        {
            newBee.onKilled += () => beeQueue.Enqueue(newBee);
            newBee.onKilled += () => beeList.Remove(newBee);
        }

        newBee.Initialize(isLeader, sprite, this);
        beeList.Add(newBee);
        if(leaderBee != null)
            beeList.ForEach(x => x.SetFlockLeader(leaderBee));
        if (!newBee.IsLeader)
            newBee.SetXDestination(newBee.transform.position);

        return newBee;
    }

    private void Update()
    {
        if (leaderBee != null)
            transform.position = leaderBee.transform.position;
        else
            transform.position = Vector3.zero;

        if(GameManager.Instance.IsGameStarted)
        {
            debugTimePass += Time.deltaTime;
            if(debugTimePass >= 4)
            {
                debugTimePass = 0;
                SpawnBee(transform.position + GetRandomXPosition(), false, mountainSprite);
            }
        }
        
    }

    private Vector3 GetRandomXPosition()
    {
        return new Vector3(Random.Range(MinPosition.position.x, MaxPosition.position.x), Random.Range(MinPosition.position.y, MaxPosition.position.y), 0);
    }

    public void Jump()
    {
        foreach (var bee in beeList)
        {
            bee.SetRandomPos(-YRandomPositionOnJump, YRandomPositionOnJump);
            bee.Jump();
        }
    }

    public void SetNewLeader(Bee bee = null)
    {
        leaderBee = bee != null 
            ? bee 
            : beeList.Count > 0 
            ? beeList.First()
            : null;

        if(leaderBee == null)
        {
            GameManager.Instance.GameOver();
            return;
        }

        leaderBee.SetLeader();
        beeList.ForEach(x => x.SetFlockLeader(leaderBee));
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if(FrontFlockPosition != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(FrontFlockPosition.position, 0.3f);

            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(FrontFlockPosition.position + new Vector3(-2, YRandomPositionOnJump), 0.2f);
            Gizmos.DrawWireSphere(FrontFlockPosition.position - new Vector3(2, YRandomPositionOnJump), 0.2f);
            Gizmos.DrawLine(FrontFlockPosition.position + new Vector3(-2, YRandomPositionOnJump), FrontFlockPosition.position - new Vector3(2, YRandomPositionOnJump));
        }

        if(MinPosition != null && MaxPosition != null)
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
#endif
}
