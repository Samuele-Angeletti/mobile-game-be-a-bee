
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKilledMessage : IMessage
{
    public EEnemyType EnemyType;

	public EnemyKilledMessage(EEnemyType enemyType)
	{
		EnemyType = enemyType;
	}
}
