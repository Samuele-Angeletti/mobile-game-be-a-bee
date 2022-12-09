using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Obstacle : MonoBehaviour, ISpawnable
{
    [SerializeField] List<Collider2D> m_Walls;
    [SerializeField] Rigidbody2D m_Rigidbody;
    [SerializeField] TextMeshProUGUI m_AmountForPassingText;
    [SerializeField] SpriteRenderer m_HelperTrackRenderer;

    private EObstacleType m_ObstacleType;
    private EMessageType m_MessageType;
    private int m_MinBirdsForPassing;
    private float m_Speed;
    private float m_XLimitOffset;
    private int m_ScoreOnPass;
    private List<Bird> m_BirdsColliding = new List<Bird>();
    private bool m_Triggered;
    private ObstacleVariant m_Variant;
    private float m_ProbabilityOfSpawn;

    public DeactiveObject OnDeactive { get; set; }
    public float Probability => m_ProbabilityOfSpawn;

    public ScriptableObject GetVariant()
    {
        return m_Variant;
    }

    private void FixedUpdate()
    {
        m_Rigidbody.velocity = Vector3.left * m_Speed * Time.fixedDeltaTime;
    }

    private void Update()
    {
        if (transform.position.x < m_XLimitOffset)
        {
            DestroyMe(0);
        }
    }

    public void Initialize(ScriptableObject variant, float xLimit, Vector3 startPosition, Quaternion startRotation)
    {
        ObstacleVariant obstacleSO = variant as ObstacleVariant;
                
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        ActiveWalls(true);

        transform.position = startPosition;
        transform.rotation = startRotation;
        
        for (int i = 0; i < m_Walls.Count; i++)
        {
            m_Walls[i].gameObject.SetActive(obstacleSO.WallsActive[i]);
            m_Walls[i].gameObject.GetComponent<SpriteRenderer>().sprite = obstacleSO.Sprite;
        }

        if (m_Variant == null)
        {
            m_Variant = obstacleSO;
            m_ObstacleType = obstacleSO.ObstacleType;
            m_MessageType = obstacleSO.MessageTypeOnPassing;
            m_MinBirdsForPassing = obstacleSO.BirdsNeededForPassingThrough;
            m_MessageType = obstacleSO.MessageTypeOnPassing;
            m_Speed = obstacleSO.Speed;
            m_XLimitOffset = xLimit;
            m_ScoreOnPass = obstacleSO.ScoreOnPass;
            m_ProbabilityOfSpawn = obstacleSO.ProbabilityToSpawn;
        }

        m_Rigidbody.velocity = Vector3.zero;
        m_BirdsColliding.Clear();

        if (m_ObstacleType == EObstacleType.Normal)
        {
            m_HelperTrackRenderer.enabled = true;
        }
        else
        {
            UpdateBirdAmountText(0);
            m_HelperTrackRenderer.enabled = false;
        }

        m_Triggered = false;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Bird b = collision.gameObject.GetComponent<Bird>();
        if (b != null)
        {
            CollisionAction(b);
        }
    }

    private void CollisionAction(Bird b)
    {
        switch (m_ObstacleType)
        {
            case EObstacleType.Normal:
                b.Kill();
                break;
            case EObstacleType.Destructible:

                if (m_BirdsColliding.Contains(b)) return;

                m_BirdsColliding.Add(b);

                b.CanMove = false;
                b.gameObject.transform.parent = transform;
                b.SetMass(false);
                UpdateBirdAmountText(m_BirdsColliding.Count);

                if (m_BirdsColliding.Count >= m_MinBirdsForPassing)
                {
                    ManageMessages(b);

                    ActiveWalls(false);
                    SpawnVFX();
                }
                break;
        }
    }

    private void ManageMessages(Bird b)
    {
        PubSub.PubSub.Publish(new ObstaclePassedMessage(m_Variant.IsBoss));

        if (m_MessageType == EMessageType.AddOneBird)
        {
            MessageSender.SendMessage(m_MessageType, b.BirdType);
        }
        else if (m_MessageType == EMessageType.ExpandWorld)
        {
            MessageSender.SendMessage(m_MessageType, m_Variant.ExpandWorldUp);
        }
        else
        {
            MessageSender.SendMessage(m_MessageType);
        }

        if (m_ScoreOnPass > 0)
        {
            MessageSender.SendMessage(m_MessageType, m_ScoreOnPass);
        }
    }

    private void SpawnVFX()
    {
        //Debug.Log("Obstacle passed - Spawning particle effects **** ");
    }

    private void ActiveWalls(bool active)
    {
        m_Walls.ForEach(x => x.gameObject.SetActive(active));
        m_AmountForPassingText.gameObject.SetActive(active);
        m_HelperTrackRenderer.enabled = active;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!m_Triggered)
        {
            Bird b = collision.gameObject.GetComponent<Bird>();
            if (b != null)
            {
                m_HelperTrackRenderer.enabled = false;
                MessageSender.SendMessage(m_MessageType, m_ScoreOnPass);
                m_Triggered = true;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (m_ObstacleType == EObstacleType.Destructible)
        {
            Bird b = collision.gameObject.GetComponent<Bird>();
            if (b != null)
            {
                if (m_BirdsColliding.Contains(b))
                {
                    m_BirdsColliding.Remove(b);
                    b.CanMove = true;
                    b.gameObject.transform.parent = null;
                    b.SetMass(true);
                    UpdateBirdAmountText(m_BirdsColliding.Count);
                }
            }
        }
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

    void UpdateBirdAmountText(int amount)
    {
        m_AmountForPassingText.text = $"{amount.ToString()}/{m_MinBirdsForPassing.ToString()}";
    }

    public GameObject GetGameObject() => gameObject;

    public float GetProbabilityToSpawn()
    {
        return Probability;
    }
}
