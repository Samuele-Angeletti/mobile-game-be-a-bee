using UnityEngine;

public class Bee : MonoBehaviour
{
    [SerializeField] GameObject _leaderPointerRenderer;
    [SerializeField] float _jumpForce;
    [SerializeField] float _jumpHeigth;
    [SerializeField] float _gravity;
    [SerializeField] float _maxSpeed;
    [SerializeField] float _minDelta;
    [SerializeField] float _forwardSpeed;
    [SerializeField] float _followSpeed;
    [SerializeField] float _slowDownDistance;

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
    private float _randomPosRelativeToLeader;
    private Bee _flockLeader;
    private FlockManager _flockManager;
    public bool IsLeader { get; set; }

    private void Awake()
    {
        _rigidbody2D = gameObject.SearchComponent<Rigidbody2D>();
        _spriteRenderer = gameObject.SearchComponent<SpriteRenderer>();

        _destinationFlap = Vector3.zero;
    }

    public void Initialize(bool isLeader, Sprite initialSprite, FlockManager flockManager)
    {
        IsLeader = isLeader;
        ChangeSprite(initialSprite);
        _flockManager = flockManager;
        _canMove = true;
    }

    public void ChangeSprite(Sprite initialSprite)
    {
        _spriteRenderer.sprite = initialSprite;
    }

    public void Kill()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
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

    public void Jump()
    {
        _jumping = true;
    }

    void FixedUpdate()
    {
        if (_canMove)
        {
            if (IsLeader)
            {
                if (!_goingUp)
                {
                    _movementVector = Vector3.zero;

                    _verticalMove += Vector2.up.y * _gravity * Time.fixedDeltaTime;

                    _verticalMove = Mathf.Clamp(_verticalMove, -_maxSpeed, _maxSpeed);

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

                    _rigidbody2D.velocity = Vector3.up * force * Time.fixedDeltaTime;

                }
            }
            else
            {
                FollowTheLeader();
            }

            if (!_onXDestination)
            {
                GoToXDestination(_xDestination);
            }
        }
        else
        {
            _rigidbody2D.velocity = Vector3.zero;
            _verticalMove = 0;
        }
    }

    private void FollowTheLeader()
    {
        Vector3 npos = _flockLeader.transform.position + new Vector3(0, _randomPosRelativeToLeader, 0);
        Vector3 direction = npos - transform.position;
        Vector3 movement = new Vector3(0, direction.normalized.y * _followSpeed * Time.fixedDeltaTime);

        if (Mathf.Abs(direction.y) > _slowDownDistance)
        {
            _rigidbody2D.velocity = movement;
        }
        else
        {
            _rigidbody2D.velocity = movement / 2;
        }
    }

    public void GoToXDestination(Vector3 destination)
    {
        Vector3 direction = destination - transform.position;
        if (Mathf.Abs(destination.x - transform.position.x) > 0.01f)
            _rigidbody2D.velocity = new Vector3(direction.normalized.x * Time.fixedDeltaTime * _forwardSpeed, _rigidbody2D.velocity.y);
        else
        {
            transform.position = new Vector3(destination.x, transform.position.y);
            _rigidbody2D.velocity = new Vector3(0, _rigidbody2D.velocity.y);
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

    public void SetRandomPos(float min, float max)
    {
        _randomPosRelativeToLeader = Random.Range(min, max);
    }

    public void SetXDestination(Vector3 newXDestination)
    {
        _xDestination = newXDestination;
    }

    public void SetFlockLeader(Bee flockLeader)
    {
        this._flockLeader = flockLeader;
    }
}