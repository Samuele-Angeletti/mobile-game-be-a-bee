using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] Transform m_PivotRight;
    [SerializeField] Transform m_PivotLeft;
    [SerializeField] float m_Speed;
    [SerializeField] bool m_OverrideStartPos;
    [SerializeField] Transform m_PivotStartPos;

    private Vector3 m_StartPos;
    private Vector3 m_Direction;
    private bool m_canMove;

    public bool CanMove
    {
        get => m_canMove;
        set
        {
            m_canMove = value;
        }
    }

    private void Start()
    {
        m_StartPos = m_OverrideStartPos ? m_PivotStartPos.position : transform.position;
        m_Direction = new Vector3(m_PivotLeft.position.x - m_StartPos.x, 0, 0);
    }

    private void Update()
    {
        if (CanMove)
        {
            transform.position += m_Direction.normalized * m_Speed * Time.deltaTime;
            if (m_PivotRight.position.x <= m_PivotLeft.position.x)
            {
                transform.position = m_StartPos;
            }
        }
    }

}
