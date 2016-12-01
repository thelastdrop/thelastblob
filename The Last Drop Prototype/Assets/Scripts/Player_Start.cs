using UnityEngine;
using System.Collections;

public class Player_Start : MonoBehaviour {

    public GameObject m_Player_Prefab;

	// Use this for initialization
	void Start ()
    {
        Instantiate(m_Player_Prefab, gameObject.transform.position, Quaternion.identity);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
