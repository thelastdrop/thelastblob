using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformChild : MonoBehaviour {

	private MovingPlatform mp;

	void Start()
	{
		mp = GetComponentInParent<MovingPlatform>();
		Debug.Log("mp " + mp.ToString());
	}

	/// <summary>
	/// Sent each frame where a collider on another object is touching
	/// this object's collider (2D physics only).
	/// </summary>
	/// <param name="other">The Collision2D data associated with this collision.</param>
	void OnCollisionStay2D(Collision2D other)
	{
		other.rigidbody.AddForce(Vector2.right * mp.m_mov_verse * mp.m_speed * 15f);
	}
}
