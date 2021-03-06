﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Console : MonoBehaviour {


    public GameObject m_Red_Light;
    public GameObject m_Green_Light;

    public bool m_Is_Gravity = false;
    public int m_Gravity_IND = 0;
    public GameObject[] object_Linked;
    [Tooltip("Time the console will take to recover, in secs")]
    public float m_Time_To_Recover;
    public float m_Update_Tick = 0.032f;

    public AudioClip m_Active_Sound;

    private float m_last_use;
    private int m_Player_Particle_Inside;

    private bool m_flipflop = false;

    // Activate only once
    private bool used = false;

	// Use this for initialization
	void Start () {
	}
	
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            if ((m_Player_Particle_Inside == 0) &&
                 (m_Is_Gravity == false))
            {
                POLIMIGameCollective.EventManager.StartListening("Shake", activate);
//                Debug.Log( "Console sente " );
            }
            if ((m_Player_Particle_Inside == 0) &&
                 (m_Is_Gravity == true))
            {
                POLIMIGameCollective.EventManager.StartListening("Shake", gravity);
            }

            m_Player_Particle_Inside++;
        }
        
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            m_Player_Particle_Inside--;

            if ((m_Player_Particle_Inside == 0) &&
                 (m_Is_Gravity == false))
            {
                POLIMIGameCollective.EventManager.StopListening("Shake", activate);
//                Debug.Log("Console sorda ");
            }
            if ((m_Player_Particle_Inside == 0) &&
                 (m_Is_Gravity == true))
            {
                POLIMIGameCollective.EventManager.StopListening("Shake", gravity);
            }

        }
    }

    void activate()
    {
        if (used == false && Time.time - m_last_use > m_Time_To_Recover)
        {
            used = true;
            if (m_Time_To_Recover > 0f) InvokeRepeating("UpDate", m_Update_Tick, m_Update_Tick);
            // Use The console!
            foreach (GameObject go in object_Linked)
            {
                // Cast to interface_console
                IConsoleIteration iconGO = (IConsoleIteration)go.GetComponent(typeof(IConsoleIteration));
                if (iconGO != null)
                {
                    SoundManager.Instance.PlayLevelSound(m_Active_Sound, false, 0f);
                    iconGO.Activate_Once();
                }
            }

            if (m_Red_Light != null)
            {
                if (m_flipflop == false) green_light();
                else red_light();
            }
            m_last_use = Time.time;
        }
    }

    void gravity()
    {
        if (used == false && Time.time - m_last_use > m_Time_To_Recover)
        {
            used = true;
            // Use The console!
            if (m_Time_To_Recover > 0f) InvokeRepeating("UpDate", m_Update_Tick, m_Update_Tick);
            GameManager.Instance.Gravity_Change( m_Gravity_IND );
            SoundManager.Instance.PlayLevelSound(m_Active_Sound, false, 0f);

            m_last_use = Time.time;
        }
    }

    void UpDate()
    {
        if (Time.time - m_last_use > m_Time_To_Recover)
        {
            used = false;
            CancelInvoke("UpDate");
        }
    }

    void red_light()
    {
        m_flipflop = true;
        m_Red_Light.SetActive(true);
        m_Green_Light.SetActive(false);
    }

    void green_light()
    {
        m_flipflop = false;
        m_Red_Light.SetActive(false);
        m_Green_Light.SetActive(true);
    }
}