using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PubSub;

public class UIManager : MonoBehaviour, ISubscriber
{
	[Header("Scene References")]
	[SerializeField] UIMainMenu m_MainMenu;
	[SerializeField] UISettingsMenu m_SettingsMenu;
	[SerializeField] UIPauseMenu m_PauseMenu;
	[SerializeField] UIGameOverMenu m_GameOverMenu;
	[SerializeField] UIMainDisplay m_MainDisplay;

	private Menu m_CurrentMenu;

	void Start()
	{
		PubSub.PubSub.Subscribe(this, typeof(OpenMenuMessage));
		PubSub.PubSub.Subscribe(this, typeof(SetWorldMessage));
		PubSub.PubSub.Subscribe(this, typeof(ExpandWorldConfineMessage));
	}

	public void OnPublish(IMessage message)
	{
		if (message is OpenMenuMessage)
		{
			OpenMenuMessage openMenu = (OpenMenuMessage)message;
			OpenMenu(openMenu.MenuType);
		}
		else if(message is ExpandWorldConfineMessage)
        {
			ExpandWorldConfineMessage expandWorldConfine = (ExpandWorldConfineMessage)message;
			string messageText = expandWorldConfine.GoingToTheSky ? "Mondo Ampliato: vola in alto!" : "Mondo Ampliato: vola in basso!";
			m_MainDisplay.SetLowMessage(messageText);
        }
		else if (message is SetWorldMessage)
		{
			SetWorldMessage setWorld = (SetWorldMessage)message;
			m_MainDisplay.SetLowMessage(WorldPivotToString(setWorld.NewPivot));
		}
	}

	private void OpenMenu(EMenu menuToOpen)
	{
		if(m_CurrentMenu == GetMenuByEnum(menuToOpen)) // se apro lo stesso, chiudo lo stesso menu
        {
			m_CurrentMenu.Close();
			m_CurrentMenu = null;
			return;
		}

		if (m_CurrentMenu != null) m_CurrentMenu.Close();

		m_CurrentMenu = GetMenuByEnum(menuToOpen);
		m_CurrentMenu.Open();
	}

	private Menu GetMenuByEnum(EMenu menu)
	{
		switch (menu)
		{
			case EMenu.Main:
				return m_MainMenu;
			case EMenu.Settings:
				return m_SettingsMenu;
			case EMenu.Pause:
				return m_PauseMenu;
			case EMenu.GameOver:
				return m_GameOverMenu;
			default:
				return null;
		}
	}

	private string WorldPivotToString(EPivot ePivot)
    {
        switch (ePivot)
        {
            case EPivot.Center:
				return "Punte innevate";
            case EPivot.UpOne:
				return "Montagne lontane";
            case EPivot.UpTwo:
				return "Cielo stellato";
            case EPivot.UpThree:
				return "Spazio aperto";
            case EPivot.DownOne:
				return "Catena montuosa";
            case EPivot.DownTwo:
				return "Colline dolci";
            case EPivot.DownThree:
				return "Pianura";
			default:
				return "";
        }
    }

    public void SetMetersOnDisplay(float amount)
	{
		amount = (int)amount;
		m_MainDisplay.SetNewMeters(amount.ToString());
	}

	public void UpdateFlockAmountDisplay(int amount)
	{
		m_MainDisplay.FlockAmount(amount);
	}

	public void SetScoreOnDisplay(float amount)
	{
		amount = (int)amount;
		m_MainDisplay.SetNewScore(amount.ToString());
	}
}