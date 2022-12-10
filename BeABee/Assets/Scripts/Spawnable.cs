using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnable : MonoBehaviour
{
    [SerializeField] ESpawnableTypes spawnableType;
    [SerializeField] float horizontalSpeed;

    public ESpawnableTypes SpawnableTypes => spawnableType;

    Rigidbody2D _rigidbody;
    Vector3 _deathPosition;

    public void Initialize(Vector3 deathPosition)
    {
        _deathPosition = deathPosition;
    }

    private void Awake()
    {
        _rigidbody = gameObject.SearchComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if(transform.position.x < _deathPosition.x)
            Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        _rigidbody.velocity = -horizontalSpeed * Time.fixedDeltaTime * transform.right;
    }
}
