using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PubSub;
public class SetWorldMessage : IMessage
{
    public EPivot NewPivot;

    public SetWorldMessage(EPivot newPivot)
    {
        NewPivot = newPivot;
    }
}
