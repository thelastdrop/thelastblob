using UnityEngine;
using System.Collections;

public class Gravity : MonoBehaviour
{

	float nextUsage;
	public float delay = 2f;
	public float gravityValue = 9.81f;

	void Start ()
	{
		ResetGravity ();
		nextUsage = Time.time + delay;
	}

	void Update ()
	{
		changeGravity ();
	}

	// Reset the gravity to point down
	public void ResetGravity ()
	{
		Physics2D.gravity = new Vector3 (0f, -gravityValue, 0f);
	}

	void changeGravity ()
	{
		
		#if UNITY_STANDALONE || UNITY_WEBPLAYER // Unity3D editor or web player

		if (Input.GetKeyDown (KeyCode.W) && Time.time > nextUsage) {
			nextUsage = Time.time + delay;
			Physics2D.gravity = new Vector3 (0f, gravityValue, 0f);
		}
		if (Input.GetKeyDown (KeyCode.A) && Time.time > nextUsage) {
			nextUsage = Time.time + delay;
			Physics2D.gravity = new Vector3 (-gravityValue, 0f, 0f);
		}
		if (Input.GetKeyDown (KeyCode.S) && Time.time > nextUsage) {
			nextUsage = Time.time + delay;
			Physics2D.gravity = new Vector3 (0f, -gravityValue, 0f);			
		}
		if (Input.GetKeyDown (KeyCode.D) && Time.time > nextUsage) {
			nextUsage = Time.time + delay;
			Physics2D.gravity = new Vector3 (gravityValue, 0f, 0f);
		}
		
		#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE // mobile controls

		#endif
	}



}
