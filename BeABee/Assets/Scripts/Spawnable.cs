using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnable : MonoBehaviour
{
    [SerializeField] ESpawnableTypes spawnableType;
    [SerializeField] float horizontalSpeed;

    public ESpawnableTypes SpawnableType => spawnableType;

    [HideInInspector] public EObstacleType ObstacleType;
    [HideInInspector] public EEnemyType EnemyType;
    [HideInInspector] public EPickableType PickableType;
    
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

    public void Kill()
    {
        onDestroySpawnable?.Invoke();

        gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        _rigidbody.velocity = -horizontalSpeed * Time.fixedDeltaTime * transform.right;
    }

}
