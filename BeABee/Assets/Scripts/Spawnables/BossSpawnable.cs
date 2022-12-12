using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;
using UnityEngine;

public class BossSpawnable : EnemySpawnable
{
    [SerializeField] PickableSpawnable LootOnDestroy;
    [SerializeField] int countToDestroyBoss;
    public Sprite UiIcon;
    public string BossName;
    public int CountToDestroy => countToDestroyBoss;
    public override void Initialize(Vector3 deathPosition)
    {
        SetCountToDestroy(countToDestroyBoss);
        base.Initialize(deathPosition);
        onDefeatEnemy += () => Publisher.Publish(new SpawnObjectMessage(LootOnDestroy));
    }

}
