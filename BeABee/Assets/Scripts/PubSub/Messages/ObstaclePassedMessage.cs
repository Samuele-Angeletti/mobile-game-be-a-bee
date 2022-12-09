using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PubSub;

public class ObstaclePassedMessage : IMessage
{
    public bool IsBoss;

    public ObstaclePassedMessage(bool isBoss)
    {
        IsBoss = isBoss;
    }
}
