using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObjectMessage : IMessage
{
    public Spawnable ObjectToSpawn;

	public SpawnObjectMessage(Spawnable objectToSpawn)
	{
		ObjectToSpawn = objectToSpawn;
	}
}
