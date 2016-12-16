using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IConsoleIteration {

    public GameObject Slab;
    public GameObject Engine;

    [Tooltip("Speed at which the door open")]
    public float m_Time;
    [Tooltip("Distance of the slab to open")]
    public float m_Distance;
    [Tooltip("Tick time, every how ofter the slab will move")]
    public float m_TickTime;
    
    private bool m_IsOpen = false;

    private float m_Start_Moving = 0.0f;
    private Transform m_Slab_tr;
    private Vector3 m_Initial_Position;
    private Vector3 m_Final_Position;

    void Start()
    {
        m_Slab_tr = Slab.GetComponent<Transform>();
        m_Initial_Position = m_Slab_tr.localPosition;
        m_Final_Position = m_Initial_Position;
        m_Final_Position.y = m_Final_Position.y + m_Distance;
    }

	public void Activate_Once()
	{
        // Open the fucking door!
        if (m_IsOpen == false)
        {
            InvokeRepeating("OpenTheDoor", m_TickTime, m_TickTime);
        }
        else
        {
            InvokeRepeating("Hodor", m_TickTime, m_TickTime);
        }
    }

    void OpenTheDoor()
    {
        m_Start_Moving += m_TickTime;
        if (m_Start_Moving >= m_Time)
        {
            m_Start_Moving = m_Time;
            CancelInvoke("OpenTheDoor");
            m_IsOpen = !m_IsOpen;
        }

        m_Slab_tr.localPosition =  Vector3.Lerp( m_Initial_Position, m_Final_Position, m_Start_Moving / m_Time);
        if (m_IsOpen) m_Start_Moving = 0f;
    }

    // HODOR! Hodor, hodor.
    void Hodor()
    {
        m_Start_Moving += m_TickTime;
        if (m_Start_Moving >= m_Time)
        {
            m_Start_Moving = m_Time;
            CancelInvoke("Hodor");
            m_IsOpen = !m_IsOpen;
        }
        m_Slab_tr.localPosition = Vector3.Lerp(m_Final_Position * 1.0001f, m_Initial_Position * 1.0001f, m_Start_Moving / m_Time);
        if (!m_IsOpen) m_Start_Moving = 0f;
    }
}
