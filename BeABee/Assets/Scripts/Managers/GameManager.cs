using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region SINGLETON
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
                if (instance != null)
                    return instance;

                GameObject go = new GameObject("GameManager");
                return go.AddComponent<GameManager>();
            }
            else
                return instance;
        }
        set
        {
            instance = value;
        }
    }
    #endregion

    [HideInInspector] public bool IsGameStarted;

    InputSystem inputSystem;
    FlockManager flockManager;
    UIManager uiManager;
    private void Awake()
    {
        
        inputSystem = new InputSystem();

        inputSystem.Player.Enable();
        inputSystem.Player.TouchScreen.performed += JumpPerformed;
        inputSystem.Player.JumpDEMO.performed += JumpPerformed;

        uiManager = FindObjectOfType<UIManager>();
        flockManager = FindObjectOfType<FlockManager>();
    }

    private void JumpPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        flockManager.Jump();
    }

    public void StartGame()
    {
        flockManager.Initialize();
        IsGameStarted = true;
    }

    public void GameOver()
    {
        IsGameStarted = false;
        uiManager.ResetMenu();
    }
}
