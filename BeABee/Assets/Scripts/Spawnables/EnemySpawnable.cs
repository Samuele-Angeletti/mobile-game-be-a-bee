using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemySpawnable : Spawnable
{
    [SerializeField] EEnemyType enemyType;
    [SerializeField] TextMeshProUGUI destroyAmount;
    [SerializeField] int honeyOnDestroy;
    [Header("On Destroy Spawn Effects")]
    [SerializeField] ParticleSystem particleSystemPrefab;
    [SerializeField] Sprite deadEnemySprite;
    [SerializeField] GameObject deadBodyprefab;
    [SerializeField] GameObject honeyGainedPrefab;

    int countToDestroy;
    int currentAttachedBees;
    List<Bee> attachedBees;

    public int HoneyOnDestroy => honeyOnDestroy;

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

        UpdateUI();
    }

    public override void UpdateSpawnable()
    {
        base.UpdateSpawnable();

        if (currentAttachedBees >= countToDestroy)
        {
            Publisher.Publish(new EnemyKilledMessage(enemyType, this));

            onDefeatEnemy?.Invoke();

            Kill();
        }
    }

    public override void Kill()
    {
        attachedBees.ForEach(x => x.transform.parent = null);

        attachedBees.Clear();

        SpawnVFX();

        base.Kill();
    }

    private void SpawnVFX()
    {
        if(particleSystemPrefab != null)
        {
            ParticleSystem particleSystem = Instantiate(particleSystemPrefab, transform.position, Quaternion.identity);
            Destroy(particleSystem.gameObject, 2f);
        }

        if(deadBodyprefab != null)
        {
            GameObject deadEnemy = Instantiate(deadBodyprefab, transform.position, Quaternion.identity);
            deadEnemy.GetComponent<Rigidbody2D>().AddForce(new Vector2(-1, 1) * 5, ForceMode2D.Impulse);
            deadEnemy.GetComponent<SpriteRenderer>().sprite = deadEnemySprite;
            Destroy(deadEnemy, 2f);
        }

        if(honeyGainedPrefab != null)
        {
            GameObject honeyGained = Instantiate(honeyGainedPrefab, transform.position, Quaternion.identity);
            honeyGained.GetComponentInChildren<TextMeshProUGUI>().text = honeyOnDestroy.ToString();
            Destroy(honeyGained, 2f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<Bee>(out var bee))
        {
            if (EnemyType != EEnemyType.Boss && bee.IsInvulnerable)
            {
                Publisher.Publish(new EnemyKilledMessage(enemyType, this));
                Kill();
            }

            if (!attachedBees.Contains(bee))
            {
                attachedBees.Add(bee);

                currentAttachedBees = bee.Attacking ? currentAttachedBees + bee.BombAttackIntensity : currentAttachedBees + 1;
                UpdateUI();

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

                currentAttachedBees = bee.Attacking ? currentAttachedBees - bee.BombAttackIntensity : currentAttachedBees - 1;
                UpdateUI();
            }
        }
    }

    public virtual void SetCountToDestroy(int amount)
    {
        countToDestroy = amount;
        UpdateUI();
    }

    private void UpdateUI()
    {
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
