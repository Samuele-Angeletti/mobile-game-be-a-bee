using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject MainMenu;
    [SerializeField] GameObject ShopMenu;
    [SerializeField] GameObject PlayArea;
    [SerializeField] GameObject GameOverMenu;
    private UIGameOver _uiGameOver;
    private UIPlayArea _uiPlayArea;

    // TODO: display game over menu with statistics then ResetMenu on press "continue button"

    private void Awake()
    {
        _uiPlayArea = PlayArea.SearchComponent<UIPlayArea>();
        _uiGameOver = GameOverMenu.SearchComponent<UIGameOver>();
    }
    public void ResetMenu()
    {
        MainMenu.SetActive(true);
        ShopMenu.SetActive(false);
        PlayArea.SetActive(false);
        GameOverMenu.SetActive(false);

        _uiPlayArea.ResetValues();
    }

    public void ShowFinalStats()
    {
        _uiGameOver.FillUpStatistics();
        GameOverMenu.SetActive(true);
        PlayArea.SetActive(false);
    }

}
