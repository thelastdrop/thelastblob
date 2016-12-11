using System.Collections;
using System.Collections.Generic;
using POLIMIGameCollective;
using UnityEngine;

public class Shake_Manager : Singleton<Shake_Manager> {
    // Counting the number of shakes the phone recieve
    public int m_Shake_No_Shakes = 0;
    // Minimum acceleration to register
    public float m_Shake_Min_Accel = 0.6f;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
