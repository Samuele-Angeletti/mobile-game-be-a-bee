using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableSpawnable : Spawnable
{
    [SerializeField] EPickableType pickableType;

    public override void Initialize(Vector3 deathPosition)
    {
        base.Initialize(deathPosition);

        PickableType = pickableType;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var bee = collision.gameObject.GetComponent<Bee>();
        if(bee != null)
        {
            bee.AddPickable(pickableType);
            Kill();
        }
    }
}

public enum EPickableType
{
    AddOneBee,
    AddTwoBees,
    AddThreeBees,
    Invincible,
    Bomb
}
