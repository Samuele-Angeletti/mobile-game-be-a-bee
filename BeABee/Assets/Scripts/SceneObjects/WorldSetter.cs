using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSetter : MonoBehaviour
{
    [SerializeField] EPivot m_PivotToSet;
    private bool m_OnThisWorld;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!m_OnThisWorld)
        {
            Bird b = collision.gameObject.GetComponent<Bird>();
            if (b != null && b.IsLeader)
            {
                m_OnThisWorld = true;
                MessageSender.SendMessage(EMessageType.SetPivot, m_PivotToSet);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (m_OnThisWorld)
        {
            Bird b = collision.gameObject.GetComponent<Bird>();
            if (b != null && b.IsLeader)
            {
                m_OnThisWorld = false;
            }
        }
    }
}
