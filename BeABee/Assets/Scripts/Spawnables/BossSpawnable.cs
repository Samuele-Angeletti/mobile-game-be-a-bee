using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawnable : EnemySpawnable
{
    [SerializeField] PickableSpawnable LootOnDestroy;
    [SerializeField] int countToDestroyBoss;

    public override void Initialize(Vector3 deathPosition)
    {
        SetCountToDestroy(countToDestroyBoss);
        base.Initialize(deathPosition);

    }

    public override void Kill()
    {
        base.Kill();

        Publisher.Publish(new SpawnObjectMessage(LootOnDestroy));
    }
}
