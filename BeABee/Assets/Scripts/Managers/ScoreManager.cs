using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PubSub;

public class ScoreManager : ISubscriber
{
	private float m_MetersOnSecond;
	private int m_Meters;
	private int m_ScoreFromMessages;
	private bool m_GameOnPlay;
	private float m_TimePassed;
	private UIManager m_UIManager;
	private int m_ObstalceDestroyedCount;
	private int m_PickablePickedCount;

	public int Score => m_ScoreFromMessages;
	public int Meters => m_Meters;
	public int PickablePicked => m_PickablePickedCount;
	public int ObstacleDestroyed => m_ObstalceDestroyedCount;

	public ScoreManager(float metersOnSecond, UIManager uiManager)
	{
		m_MetersOnSecond = metersOnSecond;
		m_UIManager = uiManager;
		PubSub.PubSub.Subscribe(this, typeof(ScoreChangeMessage));
		PubSub.PubSub.Subscribe(this, typeof(GameStartMessage));
		PubSub.PubSub.Subscribe(this, typeof(PauseGameMessage));
		PubSub.PubSub.Subscribe(this, typeof(ResumeGameMessage));
		PubSub.PubSub.Subscribe(this, typeof(GameOverMessage));
		PubSub.PubSub.Subscribe(this, typeof(ObstaclePassedMessage));
		PubSub.PubSub.Subscribe(this, typeof(PickablePickedMessage));
	}

	public void Update()
	{
		if (m_GameOnPlay)
		{
			m_TimePassed += Time.deltaTime;
			if (m_TimePassed >= m_MetersOnSecond)
			{
				m_TimePassed = 0;
				m_Meters++;
				m_UIManager.SetMetersOnDisplay(m_Meters);
			}
		}
	}

	public void OnPublish(IMessage message)
	{
		if (message is ScoreChangeMessage)
		{
			ScoreChangeMessage scoreChangeMessage = (ScoreChangeMessage)message;
			m_ScoreFromMessages += scoreChangeMessage.Score;
			m_UIManager.SetScoreOnDisplay(m_ScoreFromMessages);
		}
		else if (message is GameStartMessage || message is ResumeGameMessage)
		{
			m_GameOnPlay = true;
		}
		else if (message is PauseGameMessage)
		{
			m_GameOnPlay = false;
		}
		else if (message is GameOverMessage)
		{
			m_GameOnPlay = false;
		}
		else if (message is ObstaclePassedMessage)
		{
			m_ObstalceDestroyedCount++;
		}
		else if (message is PickablePickedMessage)
		{
			m_PickablePickedCount++;
		}
	}

}