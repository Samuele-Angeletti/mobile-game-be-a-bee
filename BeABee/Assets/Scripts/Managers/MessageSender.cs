using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PubSub;
public static class MessageSender
{
    /// <summary>
    /// Sends a message to the PubSub System. It accepts the following Messages Types: none, KillOneRandomBird, HalveFlock, InvulnerabilityAll, DoubleFlock, GameOver
    /// </summary>
    /// <param name="messageType"></param>
    public static void SendMessage(EMessageType messageType)
    {
        switch (messageType)
        {
            case EMessageType.none:
                break;
            case EMessageType.KillOneRandomBird:
                PubSub.PubSub.Publish(new KillOneRandomBirdMessage());
                break;
            case EMessageType.HalveFlock:
                PubSub.PubSub.Publish(new HalveFlockMessage());
                break;
            case EMessageType.InvulnerabilityAll:
                PubSub.PubSub.Publish(new InvulnerabilityAllMessage());
                break;
            case EMessageType.DoubleFlock:
                PubSub.PubSub.Publish(new DoubleFlockMessage());
                break;
            case EMessageType.GameOver:
                PubSub.PubSub.Publish(new GameOverMessage());
                break;
        }
    }

    /// <summary>
    /// Sends a message to the PubSub System. It accepts the following Messages Types: AddOneBird, DoubleBirdsOfSameType
    /// </summary>
    /// <param name="messageType"></param>
    /// <param name="birdType"></param>
    public static void SendMessage(EMessageType messageType, EBirdType birdType)
    {
        switch (messageType)
        {
            case EMessageType.AddOneBird:
                PubSub.PubSub.Publish(new AddBirdMessage(GameManager.Instance.GetBirdVariantByEnum(birdType), false));
                break;
            case EMessageType.DoubleBirdsOfSameType:
                PubSub.PubSub.Publish(new AddBirdMessage(GameManager.Instance.GetBirdVariantByEnum(birdType), true));
                break;
        }
    }

    /// <summary>
    /// Sends a message to the PubSub System. It accepts the following Messages Types: Score
    /// </summary>
    /// <param name="messageType"></param>
    /// <param name="score"></param>
    public static void SendMessage(EMessageType messageType, int score)
    {
        switch (messageType)
        {
            case EMessageType.Score:
                PubSub.PubSub.Publish(new ScoreChangeMessage(score));
                break;
        }
    }

    /// <summary>
    /// Sends a message to the PubSub System. It accepts the following Messages Types: ExpandWorld
    /// </summary>
    /// <param name="messageType"></param>
    /// <param name="value">If ExpandWorld: True if the world should expand up. False instead</param>
    public static void SendMessage(EMessageType messageType, bool value)
    {
        switch (messageType)
        {
            case EMessageType.ExpandWorld:
                PubSub.PubSub.Publish(new ExpandWorldConfineMessage(value));
                break;
        }
    }

    /// <summary>
    /// Sends a message to the PubSub System. It accepts the following Messages Types: SetPivot
    /// </summary>
    /// <param name="messageType"></param>
    /// <param name="pivot"></param>
    public static void SendMessage(EMessageType messageType, EPivot pivot)
    {
        switch (messageType)
        {
            case EMessageType.SetPivot:
                PubSub.PubSub.Publish(new SetWorldMessage(pivot));
                break;
        }
    }

    /// <summary>
    /// Sends a message to the PubSub System. It accepts the following Messages Types: OpenMenu
    /// </summary>
    /// <param name="messageType"></param>
    /// <param name="pivot"></param>
    public static void SendMessage(EMessageType messageType, EMenu menuToOpen)
    {
        switch (messageType)
        {
            case EMessageType.OpenMenu:
                PubSub.PubSub.Publish(new OpenMenuMessage(menuToOpen));
                break;
        }
    }
}
