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
        onDefeatEnemy += () => Publisher.Publish(new ChoosingNextScenarioMessage());
        
    }

    public override void SetCountToDestroy(int amount)
    {
        countToDestroyBoss = amount;
        base.SetCountToDestroy(amount);
    }
}
