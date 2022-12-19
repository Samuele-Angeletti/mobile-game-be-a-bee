using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Spawnable : MonoBehaviour
{
    [SerializeField] ESpawnableTypes spawnableType;
    [SerializeField] float horizontalSpeed;
    [Header("On Destroy Spawn Effects")]
    public ParticleSystem ParticleSystemPrefab;
    public Sprite DeadBodyySprite;
    public GameObject DeadBodyprefab;
    public GameObject HoneyGainedPrefab;
    public int HoneyOnDestroy;
    public ESpawnableTypes SpawnableType => spawnableType;

    [HideInInspector] public EObstacleType ObstacleType = EObstacleType.None;
    [HideInInspector] public EEnemyType EnemyType = EEnemyType.None;
    [HideInInspector] public EPickableType PickableType = EPickableType.None;
    
    public delegate void OnDestroySpawnable();
    public OnDestroySpawnable onDestroySpawnable;

    Rigidbody2D _rigidbody;
    public Rigidbody2D Rigidbody => _rigidbody;
    Vector3 _deathPosition;

    public virtual void Initialize(Vector3 deathPosition)
    {
        _deathPosition = deathPosition;
    }

    private void Awake()
    {
        _rigidbody = gameObject.SearchComponent<Rigidbody2D>();
    }

    private void Update()
    {
        UpdateSpawnable();
    }

    public virtual void UpdateSpawnable()
    {
        if (transform.position.x < _deathPosition.x)
            Kill();
    }

    public virtual void Kill()
    {
        onDestroySpawnable?.Invoke();

        gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        _rigidbody.velocity = -horizontalSpeed * Time.fixedDeltaTime * transform.right;
    }


    public virtual void SpawnVFX()
    {
        if (ParticleSystemPrefab != null)
        {
            ParticleSystem particleSystem = Instantiate(ParticleSystemPrefab, transform.position, Quaternion.identity);
            Destroy(particleSystem.gameObject, 2f);
        }

        if (DeadBodyprefab != null)
        {
            GameObject deadEnemy = Instantiate(DeadBodyprefab, transform.position, Quaternion.identity);
            deadEnemy.GetComponent<Rigidbody2D>().AddForce(new Vector2(-1, 1) * 5, ForceMode2D.Impulse);
            deadEnemy.GetComponent<SpriteRenderer>().sprite = DeadBodyySprite;
            Destroy(deadEnemy, 2f);
        }

        if (HoneyGainedPrefab != null)
        {
            GameObject honeyGained = Instantiate(HoneyGainedPrefab, transform.position, Quaternion.identity);
            honeyGained.GetComponentInChildren<TextMeshProUGUI>().text = HoneyOnDestroy.ToString();
            Destroy(honeyGained, 2f);
        }
    }

}
