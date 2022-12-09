using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject MainMenu;
    [SerializeField] GameObject ShopMenu;

    public void ResetMenu()
    {
        MainMenu.SetActive(true);
        ShopMenu.SetActive(false);
    }
}
