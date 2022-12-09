using System;
using UnityEngine;

[System.Serializable]
public class SpawnBossCondition
{
	[Tooltip("The minimum score to reach")]
	[SerializeField] public int Score;
	[Tooltip("If Optional the condition can be false, else it Must be true. Remember that at least one Optional condition must be true")]
	[SerializeField] public EConditionSelector ScoreCondition;
	[Space(10)]
	[Tooltip("The minimum distance to run across")]
	[SerializeField] public int Meters;
	[Tooltip("If Optional the condition can be false, else it Must be true. Remember that at least one Optional condition must be true")]
	[SerializeField] public EConditionSelector MetersCondition;
	[Space(10)]
	[Tooltip("The minimum quantity of birds that have benn inside the flock (not only the current)")]
	[SerializeField] public int BirdsQuantity;
	[Tooltip("If Optional the condition can be false, else it Must be true. Remember that at least one Optional condition must be true")]
	[SerializeField] public EConditionSelector BirdsQuantityCondition;
	[Space(10)]
	[Tooltip("The minimum pickable to be picked")]
	[SerializeField] public int NPickablePicked;
	[Tooltip("If Optional the condition can be false, else it Must be true. Remember that at least one Optional condition must be true")]
	[SerializeField] public EConditionSelector NPickablePickedCondition;
	[Space(10)]
	[Tooltip("The minimum obstacle to destroy")]
	[SerializeField] public int NObstacleDestroyed;
	[Tooltip("If Optional the condition can be false, else it Must be true. Remember that at least one Optional condition must be true")]
	[SerializeField] public EConditionSelector NObstacleDestroyedCondition;
	[Space(10)]
	[Tooltip("The Boss to Spawn when the conditions are met")]
	[SerializeField] public ScriptableObject Boss;

	private int[] m_OptionalConditions = new int[5];
	private int[] m_ValueToReach = new int[5];
	private int m_Indexer;
	private int m_OptionalCount;
	public void Initialize()
	{
		m_ValueToReach[0] = Mathf.Clamp(Score, 0, Score);
		m_ValueToReach[1] = Mathf.Clamp(Meters, 0, Meters);
		m_ValueToReach[2] = Mathf.Clamp(BirdsQuantity, 0, BirdsQuantity);
		m_ValueToReach[3] = Mathf.Clamp(NPickablePicked, 0, NPickablePicked);
		m_ValueToReach[4] = Mathf.Clamp(NObstacleDestroyed, 0, NObstacleDestroyed);

		for (int i = 0; i < m_OptionalConditions.Length; i++)
		{
			m_OptionalConditions[i] = -1;
		}
	}

	public bool CheckConditionMet(int score, int meters, int birdsQuantity, int pickablePicked, int obstacleDestoyed)
	{
		m_Indexer = -1;
		m_OptionalCount = 0;
		if (!CheckCondition(Score, ScoreCondition, score)) return false;
		if (!CheckCondition(Meters, MetersCondition, meters)) return false;
		if (!CheckCondition(BirdsQuantity, BirdsQuantityCondition, birdsQuantity)) return false;
		if (!CheckCondition(NPickablePicked, NPickablePickedCondition, pickablePicked)) return false;
		if (!CheckCondition(NObstacleDestroyed, NObstacleDestroyedCondition, obstacleDestoyed)) return false;

		if(m_OptionalCount > 0)
			if (!CheckOptionals()) return false;

		return true;
	}

	private bool CheckCondition(int valueCondition, EConditionSelector conditioner, int valueReached)
	{
		m_Indexer++;
		if (conditioner == EConditionSelector.Must)
		{
			if (valueReached >= valueCondition)
				return true;
		}
		else
		{
			
			m_OptionalConditions[m_Indexer] = valueReached;
			m_OptionalCount++;
			return true;
		}


		return false;
	}

	private bool CheckOptionals()
	{
		for (int i = 0; i < m_OptionalConditions.Length; i++)
		{
			if (m_OptionalConditions[i] >= m_ValueToReach[i])
			{
				return true;
			}
		}

		return false;
	}

}