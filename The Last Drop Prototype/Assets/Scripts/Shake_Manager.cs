using System.Collections;
using System.Collections.Generic;
using POLIMIGameCollective;
using UnityEngine;

public class Shake_Manager : Singleton<Shake_Manager> {

    //Every how much time the input acceleration is controlled:
    public float m_Shake_Tick = 0.03f;

    // Counting the number of shakes the phone recieve
    public int m_Shake_No_Shakes = 0;

    // Minimum acceleration to register
    public float m_Shake_Min_Accel = 0.6f;
    public float m_Avrg_Time;

    public int m_Max_Values = 10;//max number of acceleration update to calculate averange

    public Vector3 m_Current_Accel;
    public Vector3 m_Unbiased_Accel;

    private Vector3 m_Avrg_Accel;

    private Queue<Vector3> m_Que_Accels;

    void OnEnable()
    {
        InvokeRepeating("Check_Shake", m_Shake_Tick, m_Shake_Tick);
        m_Que_Accels = new Queue<Vector3>();
    }

    void OnDisable()
    {
        CancelInvoke("Check_Shake");
    }
	
	public void Check_Shake () {


        m_Current_Accel = Input.acceleration;

        m_Que_Accels.Enqueue(m_Current_Accel);
        if (m_Que_Accels.Count > m_Max_Values)
            m_Que_Accels.Dequeue();

        m_Avrg_Accel.Set(0f, 0f, 0f);

        foreach(Vector3 elem in m_Que_Accels)
        {
            m_Avrg_Accel += elem;
        }

        m_Avrg_Accel /= m_Que_Accels.Count;

        m_Unbiased_Accel = m_Current_Accel - m_Avrg_Accel;

        EventManager.TriggerEvent("Shake");

	}
}
