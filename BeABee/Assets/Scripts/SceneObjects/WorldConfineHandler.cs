using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PubSub;
using Cinemachine;
public class WorldConfineHandler : MonoBehaviour, ISubscriber
{
	[Header("Main Settings")]
	[SerializeField] float m_SingleWorldYSize;
	[SerializeField] CinemachineConfiner m_CameraConfiner;

	private PolygonCollider2D m_Collider;
	private Vector2[] m_CastedPoints;
	private EPivot m_CurrentWorld;

	void Awake()
	{
		m_Collider = GetComponent<PolygonCollider2D>();
	}

	void Start()
	{
		PubSub.PubSub.Subscribe(this, typeof(ExpandWorldConfineMessage));
		PubSub.PubSub.Subscribe(this, typeof(SetWorldMessage));
	}

	public void UpgradeWorldConfine(bool growingUpTheSky)
	{
		Vector2[] points = new Vector2[m_Collider.points.Length];
		for (int i = 0; i < m_Collider.points.Length; i++)
		{
			if (growingUpTheSky)
			{
				if (i < 2)
				{
					points[i] = new Vector2(m_Collider.points[i].x, m_Collider.points[i].y + m_SingleWorldYSize);
				}
				else
				{
					points[i] = m_Collider.points[i];
				}
			}
			else
			{
				if (i > 1)
				{
					points[i] = new Vector2(m_Collider.points[i].x, m_Collider.points[i].y - m_SingleWorldYSize);
				}
				else
				{
					points[i] = m_Collider.points[i];
				}
			}
		}

		SetPoints(points);
	}

	public void OnPublish(IMessage message)
	{
		if (message is ExpandWorldConfineMessage)
		{
			ExpandWorldConfineMessage expandWorld = (ExpandWorldConfineMessage)message;
			UpgradeWorldConfine(expandWorld.GoingToTheSky);
			string direzione = expandWorld.GoingToTheSky ? "verso l'alto" : "verso il basso";
			Debug.Log($"Mondo ampliato {direzione}");
		}
		else if (message is SetWorldMessage)
		{
			SetWorldMessage setWorld = (SetWorldMessage)message;
			m_CurrentWorld = setWorld.NewPivot;
		}
	}

	public void Confine()
	{
		Vector2[] points = m_Collider.points;
		m_CastedPoints = m_Collider.points;

		Vector2 container = new Vector2();

		GameManager.Instance.ActOnEnum(
		m_CurrentWorld,
		() => container = new Vector2(6, -6),
		() => container = new Vector2(18, 6),
		() => container = new Vector2(30, 18),
		() => container = new Vector2(42, 30),
		() => container = new Vector2(-18, -6),
		() => container = new Vector2(-30, -18),
		() => container = new Vector2(-42, -30));

		for (int i = 0; i < points.Length; i++)
		{
			if (i < 2)
				points[i].y = container.x;
			else
				points[i].y = container.y;
		}

		SetPoints(points);
	}

	public void ReversePointsToLastPositions()
	{
		if (m_CastedPoints != null)
			SetPoints(m_CastedPoints);
	}

	private void SetPoints(Vector2[] points)
	{
		m_Collider.points = points;

		m_CameraConfiner.InvalidatePathCache();
	}

}
