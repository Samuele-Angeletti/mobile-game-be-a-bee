using PubSub;
using UnityEngine;

public class Pickable : MonoBehaviour, ISpawnable
{
    private Rigidbody2D m_Rigidbody;
    private EPickableType m_PickableType;
    private int m_Score;
    private PickableVariant m_Variant;
    private float m_XLimitDestroy;
    private SpriteRenderer m_SpriteRenderer;
    private float m_ProbabilityOfSpawn;

    public DeactiveObject OnDeactive { get; set; }

    public float Probability => m_ProbabilityOfSpawn;

    public ScriptableObject GetVariant()
    {
        return m_Variant;
    }

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        m_Rigidbody.velocity = Vector3.left * m_Variant.Speed * Time.fixedDeltaTime;
    }

    private void Update()
    {
        if (transform.position.x < m_XLimitDestroy)
        {
            DestroyMe(0);
        }
    }

    public void Initialize(ScriptableObject variant, float XLimitOffset, Vector3 startPosition, Quaternion startRotation)
    {
        PickableVariant pickableSO = variant as PickableVariant;
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        transform.position = startPosition;
        transform.rotation = startRotation;
        m_Rigidbody.velocity = Vector3.zero;
        transform.position = new Vector3(transform.position.x, transform.position.y + pickableSO.RandomPosition(), 0);

        if (m_Variant == null)
        {
            m_Variant = pickableSO;

            m_Score = pickableSO.Score;
            m_XLimitDestroy = XLimitOffset;
            m_PickableType = pickableSO.PickableType;
            m_SpriteRenderer.sprite = pickableSO.Sprite;
            transform.localScale = pickableSO.InitialScale;
            m_ProbabilityOfSpawn = pickableSO.ProbabilityToSpawn;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Bird m_HitBird = collision.gameObject.GetComponent<Bird>();
        if (m_HitBird != null)
        {
            MessageSender.SendMessage(m_Variant.MessageType, m_Variant.BirdType);

            if (m_Score != 0)
                PubSub.PubSub.Publish(new ScoreChangeMessage(m_Score));

            GivePickableReason(m_HitBird);

            PubSub.PubSub.Publish(new PickablePickedMessage());

            SpawnVFX();

            DestroyMe(0);
        }
    }

    private void GivePickableReason(Bird bird)
    {
        switch (m_PickableType)
        {
            case EPickableType.Invulnerability:
                bird.StartInvulnerability(3f);
                break;
            case EPickableType.SetNewLeader:
                bird.SetLeader(true);
                break;
        }
    }

    private void SpawnVFX()
    {
        //Debug.Log("Pickable collected - Spawning particle effects **** ");
    }

    public void DestroyMe(float delayTime)
    {
        Invoke("Deactive", delayTime);
    }

    private void Deactive()
    {
        OnDeactive.Invoke(this);
        gameObject.SetActive(false);
    }

    public GameObject GetGameObject() => gameObject;

    public float GetProbabilityToSpawn()
    {
        return Probability;
    }
}
