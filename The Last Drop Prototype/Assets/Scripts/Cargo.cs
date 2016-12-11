using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cargo : MonoBehaviour, IConsoleIteration {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Activate_Once() {
        gameObject.GetComponent<Rigidbody2D>().isKinematic = false;
    }
}
