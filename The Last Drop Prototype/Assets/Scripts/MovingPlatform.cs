using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {

	[Header("Movement speed"), Range(0f, 5f)]
    public float m_speed = 0.75f;
    [Header("Movement stops at extremes for this time:")]
    public float m_pause = 1;
	private int m_mov_verse = 1;
	private float tspeed;

	private Transform tr;
    private Transform starttr;
    private Transform endtr;

	void Start () {
		tr = GetComponent<Transform>().GetChild(2);
		starttr = tr.parent.GetChild(0);
        starttr.position = tr.position;
		endtr = tr.parent.GetChild(1);
		tspeed = m_speed;

	}
	
	void FixedUpdate () {
		float step = m_speed * Time.deltaTime;
		if(m_mov_verse == 1)
		{
		  	tr.position = Vector2.MoveTowards(tr.position, endtr.position, step);
		}
		else
		{
			tr.position = Vector2.MoveTowards(tr.position, starttr.position, step);
		}

		if(tr.position == starttr.position || tr.position == endtr.position)
		{
			m_mov_verse *= -1;
			m_speed = 0;
			StartCoroutine(Wait());
		}
	}

	IEnumerator Wait()
    {
       	yield return new WaitForSeconds(m_pause);
		m_speed = tspeed;
    }

	/// <summary>
	/// Sent each frame where a collider on another object is touching
	/// this object's collider (2D physics only).
	/// </summary>
	/// <param name="other">The Collision2D data associated with this collision.</param>
	void OnCollisionStay2D(Collision2D other)
	{
		Vector2 verse = m_mov_verse == 1 ? endtr.position - tr.position : starttr.position - tr.position;
		other.rigidbody.velocity = m_speed * verse;
	}
}
