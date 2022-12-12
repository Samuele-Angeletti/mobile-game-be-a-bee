using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject MainMenu;
    [SerializeField] GameObject ShopMenu;
    [SerializeField] GameObject PlayArea;
    private UIPlayArea _uiPlayArea;
    private void Awake()
    {
        _uiPlayArea = PlayArea.SearchComponent<UIPlayArea>();
    }
    public void ResetMenu()
    {
        MainMenu.SetActive(true);
        ShopMenu.SetActive(false);
        PlayArea.SetActive(false);
        _uiPlayArea.ResetValues();
    }
}
