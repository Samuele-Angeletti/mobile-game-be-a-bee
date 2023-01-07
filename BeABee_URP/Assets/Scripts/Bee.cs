using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class Bee : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject _leaderPointerRenderer;
    [SerializeField] Image warningImage;
    [SerializeField] TextMeshProUGUI invulnerableText;
    [Header("Settings")]
    [SerializeField] float _jumpForce;
    [SerializeField] float _jumpHeigth;
    [SerializeField] float _gravity;
    [SerializeField] float _maxSpeed;
    [SerializeField] float _minDelta;
    [SerializeField] float _forwardSpeed;
    [SerializeField] float _followSpeed;
    [SerializeField] float _slowDownDistance;
    [SerializeField] SpriteRenderer invulnerableGraphics;
    [SerializeField] float speedChangeColor;

    private float _bombAttackSpeed;
    private Rigidbody2D _rigidbody2D;
    private SpriteRenderer _spriteRenderer;
    private Vector3 _destinationFlap;
    private Vector3 _movementVector;
    private bool _canMove;
    private bool _jumping;
    private bool _goingUp;
    private float _verticalMove;
    private bool _onXDestination = false;
    private Vector3 _xDestination;
    private float _randomYRelativeToLeader;
    private Bee _flockLeader;
    private FlockManager _flockManager;
    private bool _isInvulnerable;
    private float _invulnerableTimer = 0;
    private bool _blueToGreen = true;
    private bool _greenToRed = false;
    private bool _redToBlue = false;
    private bool _attacking = false;
    public bool Locked { get; private set; }
    public bool IsLeader { get; set; }
    public bool Attacking => _attacking;
    public int BombAttackIntensity { get; private set; }
    public bool CanMove => _canMove;
   
    public bool IsInvulnerable => _isInvulnerable;
    public delegate void OnKilled();
    public OnKilled onKilled;
    private int _originalLayer;
    private Vector3 _bombAttackXDestination;

    private Coroutine _bornInvulnerabilityCoroutine;
    bool _boolJustBorn;
    private void Awake()
    {
        _rigidbody2D = gameObject.SearchComponent<Rigidbody2D>();
        _spriteRenderer = gameObject.SearchComponent<SpriteRenderer>();

        _destinationFlap = Vector3.zero;

        _originalLayer = gameObject.layer;
    }

    public void Initialize(bool isLeader, Sprite initialSprite, FlockManager flockManager, Vector3 bombAttackDestination, float bornInvulnerabilityTime)
    {
        IsLeader = isLeader;
        ChangeSprite(initialSprite);
        _flockManager = flockManager;
        _canMove = true;
        _spriteRenderer.flipX = false;
        _attacking = false;
        _greenToRed = false;
        _redToBlue = false;
        _blueToGreen = true;
        _isInvulnerable = false;
        _invulnerableTimer = 0;

        if(!isLeader)
            Locked = flockManager.LeaderBee.Locked;

        _bombAttackXDestination = bombAttackDestination;

        if (_bornInvulnerabilityCoroutine == null)
            _bornInvulnerabilityCoroutine = StartCoroutine(BornInvulnerability(bornInvulnerabilityTime));
        else
        {
            StopCoroutine(_bornInvulnerabilityCoroutine);
            _bornInvulnerabilityCoroutine = StartCoroutine(BornInvulnerability(bornInvulnerabilityTime));
        }    
    }

    private IEnumerator BornInvulnerability(float time)
    {
        
        SetInvulnerable(true, 9);
        _boolJustBorn = true;
        yield return new WaitForSeconds(time);
        _boolJustBorn = false;
        SetInvulnerable(false, _originalLayer);
        _bornInvulnerabilityCoroutine = null;
    }

    public void ChangeSprite(Sprite initialSprite)
    {
        _spriteRenderer.sprite = initialSprite;
    }

    public void Kill()
    {
        onKilled?.Invoke();
        _rigidbody2D.velocity = Vector2.zero;
        _verticalMove = 0;

        UpdateInvlunerableText(false, -1);
        ActiveWarning(false);

        if (_isInvulnerable)
        {
            SetInvulnerable(false, -1);
        }

        if (IsLeader)
        {
            IsLeader = false;
            _leaderPointerRenderer.SetActive(false);
            _flockManager.SetNewLeader();
        }
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_isInvulnerable)
        {
            CycleColor();
        }

        if (Locked) return;

        if (_canMove && IsLeader)
        {
            if (_jumping)
            {
                _jumping = false;
                _goingUp = true;
                _destinationFlap = transform.position + Vector3.up * _jumpHeigth;
            }

            if (_goingUp)
            {
                if (_destinationFlap.y - transform.position.y <= _minDelta)
                {
                    _goingUp = false;
                }
            }
        }

    }

    private void CycleColor()
    {
        _invulnerableTimer += Time.deltaTime / speedChangeColor;

        if (_blueToGreen == true && _greenToRed == false && _redToBlue == false)
        {
            invulnerableGraphics.color = Color.Lerp(Color.blue, Color.green, _invulnerableTimer);
            if (_invulnerableTimer >= speedChangeColor)
            {
                _invulnerableTimer = 0;
                _blueToGreen = false;
                _greenToRed = true;
            }
        }
        else if (_greenToRed == true && _blueToGreen == false && _redToBlue == false)
        {
            invulnerableGraphics.color = Color.Lerp(Color.green, Color.red, _invulnerableTimer);
            if (_invulnerableTimer >= speedChangeColor)
            {
                _invulnerableTimer = 0;
                _greenToRed = false;
                _redToBlue = true;
            }
        }
        else if (_redToBlue == true && _greenToRed == false && _blueToGreen == false)
        {
            invulnerableGraphics.color = Color.Lerp(Color.red, Color.blue, _invulnerableTimer);
            if (_invulnerableTimer >= speedChangeColor)
            {
                _invulnerableTimer = 0;
                _redToBlue = false;
                _blueToGreen = true;
            }
        }
    }

    public void Jump()
    {
        if (Locked) return;

        if (_canMove)
            _jumping = true;
    }

    void FixedUpdate()
    {

        if (_canMove && !_attacking)
        {
            if(!Locked)
            {
                if (IsLeader)
                {
                    if (!_goingUp)
                    {
                        _verticalMove += Vector2.up.y * _gravity * Time.fixedDeltaTime;
                        _verticalMove = Mathf.Clamp(_verticalMove, -_maxSpeed, _maxSpeed);

                        _movementVector = Vector3.zero;
                        _movementVector.y = _verticalMove;

                        _rigidbody2D.velocity = _movementVector;
                    }
                    else
                    {
                        _verticalMove = 0;
                        _movementVector = Vector3.zero;

                        float delta = Vector3.Distance(transform.position, _destinationFlap);

                        if (delta < _minDelta)
                            delta = _minDelta;

                        float force = _jumpForce * delta;

                        _rigidbody2D.velocity = force * Time.fixedDeltaTime * Vector3.up;
                    }
                }
                else
                {
                    FollowTheLeader();
                }
            }

            if (!_onXDestination)
            {
                GoToXDestination(_xDestination, _forwardSpeed);
            }
        }
        else if (_canMove && _attacking)
        {
            GoToXDestination(_bombAttackXDestination, _bombAttackSpeed);
        }
    }

    private void FollowTheLeader()
    {
        Vector3 npos = _flockLeader.transform.position + new Vector3(0, _randomYRelativeToLeader, 0);
        Vector3 direction = npos - transform.position;
        Vector3 movement = new(0, direction.normalized.y * _followSpeed * Time.fixedDeltaTime);

        _rigidbody2D.velocity = Mathf.Abs(direction.y) > _slowDownDistance ? (Vector2)movement : (Vector2)(movement / 2);
    }

    public void GoToXDestination(Vector3 destination, float speed)
    {
        Vector3 direction = destination - transform.position;
        if (Mathf.Abs(destination.x - transform.position.x) > 0.01f)
            _rigidbody2D.velocity = new(direction.normalized.x * Time.fixedDeltaTime * speed, _rigidbody2D.velocity.y);
        else
        {
            transform.position = new(destination.x, transform.position.y);
            _rigidbody2D.velocity = new(0, _rigidbody2D.velocity.y);
            _onXDestination = true;
        }
    }

    public void SetLeader()
    {
        IsLeader = true;
        _onXDestination = false;
        _xDestination = _flockManager.FrontFlockPosition.position;
        _leaderPointerRenderer.SetActive(true);
    }

    public void SetRandomY(float min, float max)
    {
        _randomYRelativeToLeader = Random.Range(min, max);
    }

    public void SetXDestination(Vector3 newXDestination)
    {
        _onXDestination = false;
        _xDestination = newXDestination;
    }

    public void SetFlockLeader(Bee flockLeader)
    {
        _jumping = false;
        _goingUp = false;
        _flockLeader = flockLeader;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool isDeathWall = collision.GetComponentInParent<DeathWall>() != null;
        if (isDeathWall || collision.GetComponentInParent<ObstacleSpawnable>() != null)
        {
            if (_isInvulnerable && !isDeathWall)
                return;

            if (!_canMove)
                transform.parent = null;
            Kill();
        }
        else if (collision.TryGetComponent<EnemySpawnable>(out var enemySpawnable))
        {
            if (enemySpawnable.EnemyType != EEnemyType.Boss && _isInvulnerable)
                return;

            _rigidbody2D.velocity = enemySpawnable.Rigidbody.velocity;
            _verticalMove = 0;
            _canMove = false;
            _jumping = false;
            _goingUp = false;
            transform.parent = enemySpawnable.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<EnemySpawnable>() != null)
        {
            _canMove = true;
            transform.parent = null;

            if (Attacking)
            {
                _attacking = false;
                Kill();
            }
        }
    }

    public void SetInvulnerable(bool invulnerable, int invulnerableLayer)
    {
        if(_boolJustBorn && _bornInvulnerabilityCoroutine != null)
        {
            StopCoroutine(_bornInvulnerabilityCoroutine);
            _bornInvulnerabilityCoroutine = null;
            _boolJustBorn = false;
        }

        if (invulnerable)
        {
            gameObject.layer = invulnerableLayer;
            invulnerableGraphics.gameObject.SetActive(true);
        }
        else
        {
            gameObject.layer = _originalLayer;
            invulnerableGraphics.gameObject.SetActive(false);
        }

        _isInvulnerable = invulnerable;
    }


    public void AddPickable(EPickableType pickableType, PickableSpawnable pickableSpawnable = null)
    {
        switch (pickableType)
        {
            case EPickableType.AddOneBee:
                _flockManager.SpawnRandomBee();
                break;
            case EPickableType.AddTwoBees:
                _flockManager.SpawnRandomBee();
                _flockManager.SpawnRandomBee();
                break;
            case EPickableType.AddThreeBees:
                _flockManager.SpawnRandomBee();
                _flockManager.SpawnRandomBee();
                _flockManager.SpawnRandomBee();
                break;
            case EPickableType.Invincible:
                _flockManager.SetInvulnerableFlock();
                break;
            case EPickableType.Bomb:
                _flockManager.BombQuantity += 1;
                break;
            case EPickableType.Pollen:
                GameManager.Instance.PollenPicked += pickableSpawnable.PollenGenerated;
                break;
        }
    }

    public void BombAttack(int bombAttackIntensity, float bombAttackSpeed)
    {
        BombAttackIntensity = bombAttackIntensity;
        _bombAttackSpeed = bombAttackSpeed;
        _attacking = true;
        _spriteRenderer.flipX = true;
        _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, 0);
        if (_isInvulnerable)
        {
            SetInvulnerable(false, -1);
        }

    }

    public void UpdateInvlunerableText(bool active, int amount)
    {
        if (!active)
        {
            invulnerableText.gameObject.SetActive(false);
            return;
        }

        if (!invulnerableText.gameObject.activeSelf)
        {
            invulnerableText.gameObject.SetActive(true);
        }

        invulnerableText.text = amount.ToString();
    }

    public void ActiveWarning(bool active)
    {
        warningImage.gameObject.SetActive(active);
    }

    public void Lock(bool locked)
    {
        Locked = locked;
        _verticalMove = 0;
        _rigidbody2D.velocity = Vector2.zero;
        _goingUp = false;
        _jumping = false;
    }
}
