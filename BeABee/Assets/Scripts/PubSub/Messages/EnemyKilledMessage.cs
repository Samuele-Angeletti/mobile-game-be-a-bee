
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKilledMessage : IMessage
{
    public EEnemyType EnemyType;
    public EnemySpawnable EnemySpawnable;

    public EnemyKilledMessage(EEnemyType enemyType, EnemySpawnable enemySpawnable)
	{
		EnemyType = enemyType;
        EnemySpawnable = enemySpawnable;
	}
}
