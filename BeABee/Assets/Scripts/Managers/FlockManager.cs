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
    [Header("Specials Settings")]
    [SerializeField] int invulnerableLayer;
    [SerializeField] int invulnerableTime;
    [Space(10)]
    [SerializeField] int minBeesRequiredForBombAttack;
    [SerializeField, Range(1, 100)] int percentageUseBeeForBombAttack;
    [SerializeField] Transform bombAttackDestination;
    [SerializeField] int bombAttackIntensity = 3;
    [SerializeField] float bombAttackSpeed = 300;
    [Space(10)]
    [SerializeField] float timeDisplayWarning;
    [Space(10)]
    [SerializeField] float bornInvulnerabilityTime = 1.5f;
    private int _bombQuantity;
    public int BombQuantity
    {
        get { return _bombQuantity; }
        set { 
            _bombQuantity = value; 
            if (_uiPlayArea != null) _uiPlayArea.UpdateBombQuantity(_bombQuantity);
            if (_bombQuantity == 0) _uiPlayArea.EnableBombButton(false); 
        }
    }

    public int ActiveBeeCount => _activeBeeList.Count;

    List<Bee> _activeBeeList;
    List<Bee> BombAttackRequiredBees => _activeBeeList.Where(x => !x.IsLeader && x.CanMove).ToList();
    public Bee LeaderBee { get; private set; }
    Queue<Bee> _beeQueue;
    Coroutine _invulnerableCoroutine;
    UIPlayArea _uiPlayArea;
    private Sprite _currentSprite;
    private void Awake()
    {
        _activeBeeList = new List<Bee>();
        _beeQueue = new Queue<Bee>();
        _uiPlayArea = FindObjectOfType<UIPlayArea>();
    }

    private void Start()
    {
        FrontFlockPosition.transform.parent = null;
        MinPosition.transform.parent = null;
        MaxPosition.transform.parent = null;
        bombAttackDestination.transform.parent = null;

        GameManager.Instance.onGameOver += () => BombQuantity = 0;

        if (minBeesRequiredForBombAttack <= 1)
            minBeesRequiredForBombAttack = 2;
        Publisher.Subscribe(this, typeof(EnemyKilledMessage));
        Publisher.Subscribe(this, typeof(BossConditionMetMessage));
        Publisher.Subscribe(this, typeof(ChoosingNextScenarioMessage));
        Publisher.Subscribe(this, typeof(ScenarioChoosedMessage));
    }

    public void Initialize()
    {
        SetNewLeader(SpawnBee(FrontFlockPosition.position, true, _currentSprite));
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
            newBee.onKilled += () => _activeBeeList.Remove(newBee);
        }

        newBee.Initialize(isLeader, sprite, this, bombAttackDestination.position, bornInvulnerabilityTime);
        _activeBeeList.Add(newBee);
        if(LeaderBee != null)
            _activeBeeList.ForEach(x => x.SetFlockLeader(LeaderBee));
        if (!newBee.IsLeader)
            newBee.SetXDestination(newBee.transform.position);

        GameManager.Instance.FlockMax++;

        return newBee;
    }

    public void SpawnRandomBee()
    {
        SpawnBee(new Vector3(GetRandomXPosition().x, transform.position.y, 0), false, _currentSprite);
    }

    private void Update()
    {
        if (LeaderBee != null)
            transform.position = LeaderBee.transform.position;
        else
            transform.position = Vector3.zero;

        _uiPlayArea.EnableBombButton(BombQuantity > 0 && BombAttackRequiredBees.Count >= minBeesRequiredForBombAttack);
        
    }


    private Vector3 GetRandomXPosition()
    {
        return new Vector3(Random.Range(MinPosition.position.x, MaxPosition.position.x), 0, 0);
    }


    public void Jump()
    {
        foreach (var bee in _activeBeeList)
        {
            bee.SetRandomY(-YRandomPositionOnJump, YRandomPositionOnJump);
            bee.Jump();
        }
    }

    public void SetNewLeader(Bee bee = null)
    {
        LeaderBee = bee != null 
            ? bee 
            : _activeBeeList.Count > 0 
            ? _activeBeeList.First()
            : null;

        if(LeaderBee == null)
        {
            GameManager.Instance.GameOver();
            return;
        }

        LeaderBee.SetLeader();
        _activeBeeList.ForEach(x => x.SetFlockLeader(LeaderBee));
    }

    public void SetInvulnerableFlock()
    {
        if (_invulnerableCoroutine == null)
            _invulnerableCoroutine = StartCoroutine(InvulnerableCoroutine());
        else
        {
            StopCoroutine(_invulnerableCoroutine);
            _invulnerableCoroutine = StartCoroutine(InvulnerableCoroutine());
        }
    }

    private IEnumerator InvulnerableCoroutine()
    {
        GameManager.Instance.InvulnerabilityPicked++;
        _activeBeeList.ForEach(x => x.SetInvulnerable(true, invulnerableLayer));
        int i = invulnerableTime;
        while(i > 0)
        {
            LeaderBee.UpdateInvlunerableText(true, i);
            yield return new WaitForSeconds(1);
            i--;
        }
        LeaderBee.UpdateInvlunerableText(false, 0);
        _activeBeeList.ForEach(x => x.SetInvulnerable(false, -1));
    }

    public void UseBomb()
    {
        if (!GameManager.Instance.IsGamePlaying)
            return;

        if(BombQuantity > 0 && BombAttackRequiredBees.Count >= minBeesRequiredForBombAttack)
        {
            BombQuantity--;
            GameManager.Instance.BombUsed++;
            int beeToUse = BombAttackRequiredBees.Count * percentageUseBeeForBombAttack / 100;
            List<Bee> selectedBees = new List<Bee>();
            while (selectedBees.Count < beeToUse)
            {
                var randomBee = BombAttackRequiredBees[Random.Range(0, BombAttackRequiredBees.Count)];
                if (!selectedBees.Contains(randomBee))
                {
                    selectedBees.Add(randomBee);
                }
            }
            selectedBees.ForEach(x => x.BombAttack(bombAttackIntensity, bombAttackSpeed));
        }
    }

    public void LockFlock(bool locked)
    {
        _activeBeeList.ForEach(x => x.Lock(locked));
    }

    public void OnPublish(IMessage message)
    {
        if(message is EnemyKilledMessage)
        {
            LeaderBee.SetXDestination(FrontFlockPosition.position);
            foreach (Bee bee in _activeBeeList.Where(x => !x.IsLeader))
            {
                bee.SetXDestination(GetRandomXPosition());
            }
        }
        else if(message is BossConditionMetMessage)
        {
            LeaderBee.ActiveWarning(true);
            StartCoroutine(WarningMessageCoroutine());
        }
        else if(message is ChoosingNextScenarioMessage)
        {
            LockFlock(true);
        }
        else if(message is ScenarioChoosedMessage)
        {
            LockFlock(false);
        }
    }

    public void UpdateSprite(EScenario scenario)
    {
        switch (scenario)
        {
            case EScenario.Space:
                _currentSprite = spaceSprite;
                break;
            case EScenario.Sky:
                _currentSprite = skySprite;
                break;
            case EScenario.Mountain:
                _currentSprite = mountainSprite;
                break;
            case EScenario.Forest:
                _currentSprite = forestSprite;
                break;
            case EScenario.Underground:
                _currentSprite = undergroundSprite;
                break;
        }
    }

    private IEnumerator WarningMessageCoroutine()
    {
        var currentLeader = LeaderBee;
        yield return new WaitForSeconds(timeDisplayWarning);
        if(currentLeader.gameObject.activeSelf)
            currentLeader.ActiveWarning(false);
    }

    private void OnDisable()
    {
        Publisher.Unsubscribe(this, typeof(EnemyKilledMessage));
        Publisher.Unsubscribe(this, typeof(BossConditionMetMessage));
        Publisher.Unsubscribe(this, typeof(ChoosingNextScenarioMessage));
        Publisher.Unsubscribe(this, typeof(ScenarioChoosedMessage));

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

            if(bombAttackDestination != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(bombAttackDestination.position, 0.3f);
            }
        }
        
    }
#endif
}
