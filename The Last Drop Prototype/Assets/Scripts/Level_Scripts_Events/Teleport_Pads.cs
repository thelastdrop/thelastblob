using UnityEngine;
using System.Collections;

public class Teleport_Pads : MonoBehaviour
{

    public GameObject m_Other_Teleport_Pad;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	    //
	}

    void OnTriggerEnter2D( Collider2D coll )
    {
        // check if it's a player "ball"
        if( coll.tag == "Player")
        {
//            Debug.Log("Player detected");
        }
               // Send the player to the other teleport pad
    }
}
