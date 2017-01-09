using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobOnUfo : MonoBehaviour {

    public float m_Oscillation_Abs_x = 8.8f;
    public float m_Oscillation_Abs_y = 4.8f;
    public float m_Frequency_x = 1.8f;
    public float m_Frequency_y = 0.8f;

    private Vector3 m_screen_location;
    

	// Use this for initialization
	void Start () {
        m_screen_location = gameObject.transform.position;
	}
	

	// Update is called once per frame
	void Update () {
        
        Vector3 np = new Vector3();
        np.x = Mathf.Cos( Time.time / m_Frequency_x ) * m_Oscillation_Abs_x;
        np.y = Mathf.Sin( Time.time / m_Frequency_y ) * m_Oscillation_Abs_y;
        np.z = transform.position.z;
        gameObject.transform.position = np + m_screen_location;
    }
}
