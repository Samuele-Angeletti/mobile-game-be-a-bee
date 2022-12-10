using System.Collections;
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
    public override void Initialize(Vector3 deathPosition)
    {
        base.Initialize(deathPosition);
        attachedBees = new List<Bee>();
        EnemyType = enemyType;

        switch (enemyType)
        {
            case EEnemyType.TwoBees:
                countToDestroy = 2;
                break;
            case EEnemyType.ThreeBees:
                countToDestroy = 3;
                break;
            case EEnemyType.FourBees:
                countToDestroy = 4;
                break;
            case EEnemyType.FiveBees:
                countToDestroy = 5;
                break;
            case EEnemyType.SixBees:
                countToDestroy = 6;
                break;
            case EEnemyType.SevenBees:
                countToDestroy = 7;
                break;
        }

        destroyAmount.text = $"{currentAttachedBees}/{countToDestroy}";
    }

    public override void UpdateSpawnable()
    {
        base.UpdateSpawnable();

        if (currentAttachedBees >= countToDestroy)
        {
            attachedBees.Clear();
            Kill();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var bee = collision.gameObject.GetComponent<Bee>();
        if (bee != null)
        {
            if(!attachedBees.Contains(bee))
            {
                attachedBees.Add(bee);
                currentAttachedBees++;
                destroyAmount.text = $"{currentAttachedBees}/{countToDestroy}";
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var bee = collision.gameObject.GetComponent<Bee>();
        if (bee != null && currentAttachedBees > 0)
        {
            if(attachedBees.Contains(bee))
            {
                attachedBees.Remove(bee);
                currentAttachedBees--;
                destroyAmount.text = $"{currentAttachedBees}/{countToDestroy}";
            }
        }
    }
}

public enum EEnemyType
{
    TwoBees,
    ThreeBees,
    FourBees,
    FiveBees,
    SixBees,
    SevenBees
}
