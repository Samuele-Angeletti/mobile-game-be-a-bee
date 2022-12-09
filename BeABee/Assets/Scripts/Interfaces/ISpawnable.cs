using UnityEngine;
using System;

public delegate void DeactiveObject(ISpawnable spawnable);

public interface ISpawnable
{
	public ScriptableObject GetVariant();
	public void DestroyMe(float delayTime);
	public void Initialize(ScriptableObject variant, float XLimitOffset, Vector3 startPosition, Quaternion startRotation);
	public GameObject GetGameObject();
	DeactiveObject OnDeactive { get; set; }
	public float GetProbabilityToSpawn();
}
