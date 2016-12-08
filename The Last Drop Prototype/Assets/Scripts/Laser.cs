using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour {
	
	[Header("Start point, End point of laser")]
	public Vector2 start;
	public Vector2 end;
	[Header("Width of beam"), Range(.01f,1f)]
	public float width = .05f;
	[Header("Move speed")]
	public float m_speed;
	[Header("Offeset movement")]
	public float m_offset;
	private int m_direction = 1;

	private LineRenderer lr;

	// Use this for initialization
	void Start () {
		lr = gameObject.GetComponent<LineRenderer>();
		lr.SetColors(Color.red, Color.red);
		lr.startWidth = width;
		lr.endWidth = width;
		lr.SetPositions(new Vector3[] { new Vector3(start.x, start.y, 0f), new Vector3(end.x, end.y, 0f) });
		lr.enabled = true;
	}
	
	// Update is called once per frame
	void Update () {
		ActiveLaser();
	}

	void FixedUpdate () {
		// Move();
	}

	void ActiveLaser() {
		RaycastHit2D[] hits = Physics2D.LinecastAll(start, end);
        bool hit = false;

        foreach(RaycastHit2D elem in hits) {
			GameObject elemGO = elem.collider.gameObject;
			if(elemGO.tag == "Player") {
				hit = true;
				elemGO.SetActive(false);
			}
        }
		hit = false;
	}

	void Move() {
		if(start.x == end.x) {
			start = new Vector2(start.x * m_direction * m_speed * Time.fixedDeltaTime, start.y);
			end = new Vector2(end.x * m_direction * m_speed * Time.fixedDeltaTime, end.y);
		}
		if(start.y == end.y) {
			start = new Vector2(start.x, start.y * m_direction * m_speed * Time.fixedDeltaTime);
			end = new Vector2(end.x, end.y * m_direction * m_speed * Time.fixedDeltaTime);
		} 
	}
}
