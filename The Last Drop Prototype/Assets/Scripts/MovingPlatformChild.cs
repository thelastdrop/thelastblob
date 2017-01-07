using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformChild : MonoBehaviour {

	private MovingPlatform mp;
	[Range(0f,10f)]
	public float factor;

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
		float mulfactor = mp.m_speed >= 1 ? mp.m_speed * 4f + 10f : mp.m_speed * 4.2f + 8f;
		other.rigidbody.velocity += Vector2.right * mp.m_mov_verse * mp.m_speed * factor;
	}
}
