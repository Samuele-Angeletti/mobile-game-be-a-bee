using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PubSub;
using TMPro;

public class UIMainDisplay : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI m_MetersText;
	[SerializeField] TextMeshProUGUI m_FlockAmountText;
	[SerializeField] TextMeshProUGUI m_ScoreText;
	[SerializeField] TextMeshProUGUI m_LowMessageText;
	[SerializeField] float m_LowMessageTimeDisplay;

	private Coroutine m_TimedLowMessageCoroutine;

	int m_FlockAmount;
	private string m_LastMessage;

    private void Start()
    {
		SetNewMeters("0");
		FlockAmount(0);
		SetNewScore("0");
    }

    public void SetNewMeters(string metersString)
	{
		m_MetersText.text = "METERS: " + metersString;
	}

	public void FlockAmount(int amount)
	{
		if (amount > m_FlockAmount)
		{
			// green animation
		}
		else
		{
			// red animation
		}

		m_FlockAmount = amount;
		m_FlockAmountText.text = "FLOCK: " + amount.ToString();
	}

	public void SetNewScore(string scoreString)
	{
		m_ScoreText.text = "SCORE: " + scoreString;
	}

	public void SetLowMessage(string message)
    {
		if (m_LastMessage == message) return;

		m_LastMessage = message;

		if(m_TimedLowMessageCoroutine != null)
        {
			StopCoroutine(m_TimedLowMessageCoroutine);
        }

		m_TimedLowMessageCoroutine = StartCoroutine(TimedMessage(m_LowMessageText, message, m_LowMessageTimeDisplay));
    }

	private IEnumerator TimedMessage(TextMeshProUGUI textMesh, string message, float time)
    {
		textMesh.text = message;
		yield return new WaitForSeconds(time);
		textMesh.text = "";
    }
}