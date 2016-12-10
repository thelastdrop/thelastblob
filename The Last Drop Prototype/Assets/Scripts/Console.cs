using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Console : MonoBehaviour {

    public GameObject[] Object_Linked;

    private int m_Player_Particle_Inside;

	// Use this for initialization
	void Start () {
		
	}
	
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")    
        {
            if( m_Player_Particle_Inside == 0 )
            {
                POLIMIGameCollective.EventManager.StartListening( "PlayerShaking" , activate);
            }

            m_Player_Particle_Inside++;
        }
        
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            m_Player_Particle_Inside--;

            if (m_Player_Particle_Inside == 0)
            {
                POLIMIGameCollective.EventManager.StopListening("PlayerShaking", activate);
            }

            //Debug.Log( m_Player_Particle_Inside );
        }
    }

    void activate()
    {
        Debug.Log("Shake registered in console!");
    }
}
