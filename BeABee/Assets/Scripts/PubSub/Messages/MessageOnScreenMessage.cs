using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PubSub;
public class MessageOnScreenMessage : IMessage
{
    public string Message;

    public MessageOnScreenMessage(string message)
    {
        Message = message;
    }
}
