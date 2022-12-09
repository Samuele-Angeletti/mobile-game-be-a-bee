using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Bird : MonoBehaviour
{

    [SerializeField] SpriteRenderer m_SpriteRenderer;
    [SerializeField] Rigidbody2D m_RigidBody;
    [SerializeField] Animator m_Animator;
    [SerializeField] GameObject m_LeaderPointerRenderer;

    private BirdVariant m_Variant;
    private FlockHandler m_Flock;
    private EBirdType m_BirdType;
    private bool m_Leader;

    // movement settings
    private Vector3 m_DestinationFlap;
    private Vector3 m_MovementVector;
    private bool m_CanMove;
    private bool m_Jump;
    private bool m_GoingUp;
    private float m_JumpHeigth;
    private float m_JumpForce;
    private float m_VerticalMove;
    private float m_Gravity;
    private float m_MaxSpeed;
    private float m_MinDelta;
    private float m_XSpeed;
    private float m_FollowSpeed;
    private float m_SlowDownDistance;
    private Vector3 m_FrontFlockPosition;
    private bool m_OnXDestination = false;
    private Vector3 m_XDestination;
    private float m_RandomPosRelativeToLeader;
    private int m_BaseLayer;
    // invulnerability settings
    private bool m_Invulnerable;
    private Coroutine m_InvulnerabilityCoroutine;
    private float m_OriginalMass;
    public delegate void KillBird(Bird bird);

    public KillBird OnKillBird;
    public bool IsLeader => m_Leader;
    public EBirdType BirdType => m_BirdType;
    public bool OnXDestination
    {
        get => m_OnXDestination;
        set
        {
            m_OnXDestination = value;
        }
    }
    public bool CanMove
    {
        get => m_CanMove;
        set
        {
            m_CanMove = value;
        }
    }
    public Vector3 XDestination
    {
        get => m_XDestination;
        set
        {
            m_XDestination = value;
        }
    }
    void Awake()
    {
        m_DestinationFlap = Vector3.zero;
        m_BaseLayer = gameObject.layer;
    }

    public void Initialize(BirdVariant variant, FlockHandler flockHandler, Vector3 startPosition, Quaternion startRotation)
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        transform.position = startPosition;
        transform.rotation = startRotation;

        m_RigidBody.velocity = Vector3.zero;
        m_Leader = false;
        m_VerticalMove = 0;
        m_Jump = false;
        m_GoingUp = false;
        m_DestinationFlap = Vector3.zero;
        m_BaseLayer = gameObject.layer;
        m_Invulnerable = false;
        m_LeaderPointerRenderer.SetActive(false);

        m_Variant = variant;
        m_SpriteRenderer.sprite = m_Variant.MainSprite;
        transform.localScale = new Vector3(m_Variant.StartScale, m_Variant.StartScale);
        m_JumpForce = m_Variant.JumpForce;
        m_Flock = flockHandler;
        m_BirdType = variant.BirdType;
        m_JumpHeigth = m_Variant.JumpHeigth;
        m_Gravity = variant.Gravity;
        m_MaxSpeed = variant.MaxSpeed;
        m_MinDelta = variant.MinDelta;
        m_FollowSpeed = variant.FollowSpeed;
        m_SlowDownDistance = variant.SlowDownDistance;
        m_FrontFlockPosition = m_Flock.FrontFlockPosition;
        m_XDestination = transform.position;
        m_XSpeed = variant.XSpeed;
        m_OriginalMass = m_RigidBody.mass;
        m_CanMove = true;

    }

    internal void StartInvulnerability(float time)
    {
        m_Invulnerable = true;
        if (m_InvulnerabilityCoroutine == null)
            m_InvulnerabilityCoroutine = StartCoroutine(Invulnerability(time));
    }

    private IEnumerator Invulnerability(float time)
    {
        gameObject.layer = 10;
        yield return new WaitForSeconds(time);
        gameObject.layer = m_BaseLayer;
        m_Invulnerable = false;
        m_InvulnerabilityCoroutine = null;
    }

    internal void Kill()
    {
        if (m_Invulnerable) return;

        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (m_CanMove)
        {
            if (IsLeader)
            {
                if (m_Jump)
                {
                    m_Jump = false;
                    m_GoingUp = true;
                    m_DestinationFlap = transform.position + Vector3.up * m_JumpHeigth;
                }

                if (m_GoingUp)
                {
                    if (m_DestinationFlap.y - transform.position.y <= m_MinDelta)
                    {
                        m_GoingUp = false;
                    }
                }

            }
        }
    }

    public void Jump()
    {
        m_Jump = true;
    }

    void FixedUpdate()
    {
        if (m_CanMove)
        {
            if (IsLeader)
            {
                if (!m_GoingUp)
                {
                    m_MovementVector = Vector3.zero;

                    m_VerticalMove += Vector2.up.y * m_Gravity * Time.fixedDeltaTime;

                    m_VerticalMove = Mathf.Clamp(m_VerticalMove, -m_MaxSpeed, m_MaxSpeed);

                    m_MovementVector.y = m_VerticalMove;

                    m_RigidBody.velocity = m_MovementVector;
                }
                else
                {
                    m_VerticalMove = 0;

                    m_MovementVector = Vector3.zero;

                    float delta = Vector3.Distance(transform.position, m_DestinationFlap);

                    if (delta < m_MinDelta)
                        delta = m_MinDelta;

                    float force = m_JumpForce * delta;

                    m_RigidBody.velocity = Vector3.up * force * Time.fixedDeltaTime;

                }
            }
            else
            {
                FollowTheLeader();
            }

            if (!OnXDestination)
            {
                GoToXDestination(m_XDestination);
            }
        }
        else
        {
            m_RigidBody.velocity = Vector3.zero;
            m_VerticalMove = 0;
        }
    }

    private void FollowTheLeader()
    {
        Vector3 npos = m_Flock.FlockLeader.transform.position + new Vector3(0, m_RandomPosRelativeToLeader, 0);
        Vector3 direction = npos - transform.position;
        Vector3 movement = new Vector3(0, direction.normalized.y * m_FollowSpeed * Time.fixedDeltaTime);

        if (Mathf.Abs(direction.y) > m_SlowDownDistance)
        {
            m_RigidBody.velocity = movement;
        }
        else
        {
            m_RigidBody.velocity = movement / 2;
        }
    }

    public void GoToXDestination(Vector3 destination)
    {
        Vector3 direction = destination - transform.position;
        if (Mathf.Abs(destination.x - transform.position.x) > 0.01f)
            m_RigidBody.velocity = new Vector3(direction.normalized.x * Time.fixedDeltaTime * m_XSpeed, m_RigidBody.velocity.y);
        else
        {
            transform.position = new Vector3(destination.x, transform.position.y);
            m_RigidBody.velocity = new Vector3(0, m_RigidBody.velocity.y);
            m_OnXDestination = true;
        }
    }

    public void SetLeader(bool change)
    {
        m_Leader = change;
        OnXDestination = false;
        m_XDestination = m_FrontFlockPosition;
        m_LeaderPointerRenderer.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        WorldConfineHandler worldConfine = collision.gameObject.GetComponent<WorldConfineHandler>();
        if (worldConfine != null)
        {
            Kill();
        }
    }

    public void SetRandomPos(float min, float max)
    {
        m_RandomPosRelativeToLeader = UnityEngine.Random.Range(min, max);
    }

    void OnDisable()
    {
        OnKillBird.Invoke(this);
    }

    public void SetMass(bool active)
    {
        m_RigidBody.mass = active ? m_OriginalMass : 0;

        m_Animator.SetBool("Flapping", active);
    }
}
