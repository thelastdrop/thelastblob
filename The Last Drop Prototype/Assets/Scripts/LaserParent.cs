using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserParent : MonoBehaviour {

	[Header("Movement speed"), Range(0f,5f)]
	public float m_speed;
	[Header("Movement offset")]
	public float m_offset;
	[Header("Move Along X axis, Y axis, only one should be 1, other 0)")]
	public int alongX;
	public int alongY;
	

	private Transform tr;
	private Transform child1tr;
	private Transform child2tr;
	private Vector2 starting_pos;
	
	void Start () {
		tr = gameObject.GetComponent<Transform>() as Transform;
		child1tr = tr.GetChild(0);
		child2tr = tr.GetChild(1);
		starting_pos = child1tr.position.x == child2tr.position.x ? new Vector2(tr.position.x, 0) : new Vector2(0, tr.position.y);
	}

	void Update () {
		
	}

	void FixedUpdate() {

		// Invert direction of movement if offset is reached
		if(starting_pos.x != 0 && (tr.position.x == (starting_pos.x + m_offset) || tr.position.x == starting_pos.x)) {
			alongX *= -1;
		} else if(starting_pos.y != 0 && (tr.position.y == (starting_pos.y + m_offset) || tr.position.y == starting_pos.y)) {
			alongY *= -1;
		}
		child1tr.position = child1tr.position + alongX * m_speed * transform.right * Time.fixedDeltaTime + alongY * m_speed * transform.up * Time.fixedDeltaTime;
		child2tr.position = child2tr.position + alongX * m_speed * transform.right * Time.fixedDeltaTime + alongY * m_speed * transform.up * Time.fixedDeltaTime;
	}
}
