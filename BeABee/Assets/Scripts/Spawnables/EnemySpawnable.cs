using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemySpawnable : Spawnable
{
    [SerializeField] EEnemyType enemyType;
    [SerializeField] TextMeshProUGUI destroyAmount;

    int countToDestroy;
    int currentAttachedBees;
    List<Bee> attachedBees;

    public delegate void OnDefeatEnemy();
    public OnDefeatEnemy onDefeatEnemy;

    public override void Initialize(Vector3 deathPosition)
    {
        base.Initialize(deathPosition);
        attachedBees = new List<Bee>();
        EnemyType = enemyType;
        switch (enemyType)
        {
            case EEnemyType.TwoBees:
                SetCountToDestroy(2);
                break;
            case EEnemyType.ThreeBees:
                SetCountToDestroy(3);
                break;
            case EEnemyType.FourBees:
                SetCountToDestroy(4);
                break;
            case EEnemyType.FiveBees:
                SetCountToDestroy(5);
                break;
            case EEnemyType.SixBees:
                SetCountToDestroy(6);
                break;
            case EEnemyType.SevenBees:
                SetCountToDestroy(7);
                break;
        }

        destroyAmount.text = $"{currentAttachedBees}/{countToDestroy}";
    }

    public override void UpdateSpawnable()
    {
        base.UpdateSpawnable();

        if (currentAttachedBees >= countToDestroy)
        {
            Publisher.Publish(new EnemyKilledMessage(enemyType));

            onDefeatEnemy?.Invoke();

            Kill();
        }
    }

    public override void Kill()
    {
        attachedBees.ForEach(x => x.transform.parent = null);

        attachedBees.Clear();

        base.Kill();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<Bee>(out var bee))
        {
            if (EnemyType != EEnemyType.Boss && bee.IsInvulnerable)
            {
                Publisher.Publish(new EnemyKilledMessage(enemyType));
                Kill();
            }

            if (!attachedBees.Contains(bee))
            {
                attachedBees.Add(bee);
                currentAttachedBees++;
                destroyAmount.text = $"{currentAttachedBees}/{countToDestroy}";

                if (bee.Attacking)
                    currentAttachedBees++;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var bee = collision.gameObject.GetComponent<Bee>();
        if (bee != null && currentAttachedBees > 0)
        {
            if (attachedBees.Contains(bee))
            {
                attachedBees.Remove(bee);
                currentAttachedBees--;
                destroyAmount.text = $"{currentAttachedBees}/{countToDestroy}";

                if (bee.Attacking)
                    currentAttachedBees--;
            }
        }
    }

    public virtual void SetCountToDestroy(int amount)
    {
        countToDestroy = amount;
        destroyAmount.text = $"{currentAttachedBees}/{countToDestroy}";
    }
}

public enum EEnemyType
{
    None,
    TwoBees,
    ThreeBees,
    FourBees,
    FiveBees,
    SixBees,
    SevenBees,
    Boss
}
