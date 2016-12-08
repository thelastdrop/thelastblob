using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeBallTest : MonoBehaviour {

	Transform tr;
	Vector2 swipeVector = Vector2.zero;
	public float m_speed = .1f;


	void Start () {
		tr = GetComponent<Transform>() as Transform;
	}
	
	void Update () {
		if(SwipeManager.Instance.IsSwiping()) {
			swipeVector = SwipeManager.Instance.GetSwipeVector();
		} else {
			swipeVector = Vector2.zero;
		}
	}

	void FixedUpdate() {
		tr.position = tr.position + swipeVector.x * m_speed * transform.right * Time.fixedDeltaTime + swipeVector.y * m_speed * transform.up * Time.fixedDeltaTime;
	}
}
