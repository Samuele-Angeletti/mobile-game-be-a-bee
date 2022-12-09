using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PubSub;
public class BackgroundManager : MonoBehaviour, ISubscriber
{
    private List<Parallax> m_ParallaxObjectsList = new List<Parallax>();

    public void OnPublish(IMessage message)
    {
        if(message is GameStartMessage)
        {
            m_ParallaxObjectsList.ForEach(x => x.CanMove = true);
        }
        else if(message is GameOverMessage)
        {
            m_ParallaxObjectsList.ForEach(x => x.CanMove = false);
        }

    }

    private void Awake()
    {
        foreach(Parallax p in transform.GetComponentsInChildren<Parallax>())
        {
            m_ParallaxObjectsList.Add(p);
        }

        PubSub.PubSub.Subscribe(this, typeof(GameStartMessage));
        PubSub.PubSub.Subscribe(this, typeof(GameOverMessage));
    }


}
