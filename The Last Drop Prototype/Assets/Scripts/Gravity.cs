using UnityEngine;
using System.Collections;

public class Gravity : MonoBehaviour {

	float nextUsage;
	public float delay = 0.1f;
	public float gravityValue = 9.81f;
	Vector3 gravity;
 
	void Start() {
		gravity = Physics2D.gravity;
   		nextUsage = Time.time + delay;
 	}

	void Update () {
		changeGravity();
	}

	void FixedUpdate() {
		Physics2D.gravity = gravity;
	}

	void changeGravity() {
		if(Input.GetKeyDown (KeyCode.W) && Time.time > nextUsage) {
     		nextUsage = Time.time + delay;
			gravity.y = gravityValue;
			Debug.Log("Grav: " + Physics2D.gravity);
		}
		if(Input.GetKeyDown (KeyCode.A) && Time.time > nextUsage) {
     		nextUsage = Time.time + delay;
			gravity.x = -gravityValue;
			Debug.Log("Grav: " + Physics2D.gravity);
		}
		if(Input.GetKeyDown (KeyCode.S) && Time.time > nextUsage) {
     		nextUsage = Time.time + delay;
			gravity.y = -gravityValue;
			Debug.Log("Grav: " + Physics2D.gravity);
		}
		if(Input.GetKeyDown (KeyCode.D) && Time.time > nextUsage) {
     		nextUsage = Time.time + delay;
			gravity.x = gravityValue;
			Debug.Log("Grav: " + Physics2D.gravity);
		}
	}
}
