using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using POLIMIGameCollective;

public class DestroyOnNewScene : MonoBehaviour {

	// Use this for initialization
	void Start () {
        POLIMIGameCollective.EventManager.StartListening("DestroyOnNewScene", DestroySelf);
	}

    void DestroySelf()
    {
        Destroy( gameObject );
    }
}
