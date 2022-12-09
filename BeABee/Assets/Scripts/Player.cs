using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PubSub;
using UnityEngine.InputSystem;
using System;

public class Player : MonoBehaviour, ISubscriber
{
    [Header("Scene References")]
    [SerializeField] FlockHandler m_Flock;

    void Start()
    {
        GameManager.Instance.Inputs.Player.JumpDEMO.performed += Jump_performed;
        GameManager.Instance.Inputs.Player.TouchScreen.performed += Jump_performed;
        //GameManager.Instance.Inputs.Player.tou.performed += Jump_performed;
        PubSub.PubSub.Subscribe(this, typeof(GameOverMessage));
        Debug();
    }


    public void Debug()
    {

        Invoke("DebugginThree", 1f);
        Invoke("Debuggin", 2f);
        //Invoke("DebugginTwo", 0);
    }

    private void Jump_performed(InputAction.CallbackContext obj)
    {
        m_Flock.Jump();
    }
    void Update()
    {
        transform.position = m_Flock.GetLeaderPosition();
    }

    private void Debuggin()
    {
        PubSub.PubSub.Publish(new AddBirdMessage(GameManager.Instance.GetBirdVariantByEnum(EBirdType.Normal), false));
    }

    private void DebugginTwo()
    {
        PubSub.PubSub.Publish(new ExpandWorldConfineMessage(false));
    }

    private void DebugginThree()
    {
        PubSub.PubSub.Publish(new GameStartMessage());
    }

    public void OnPublish(IMessage message)
    {
        if (message is GameOverMessage)
        {
            //Debug();
            UnityEngine.Debug.Log("Gioco finito");
        }
    }
}
