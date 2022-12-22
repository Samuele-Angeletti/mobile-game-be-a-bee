using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossConditionChangedMessage : IMessage
{
    public BossCondition BossCondition;

	public BossConditionChangedMessage(BossCondition bossCondition)
	{
		BossCondition = bossCondition;
	}
}
