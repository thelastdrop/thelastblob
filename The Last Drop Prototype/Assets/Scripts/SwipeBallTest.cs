using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeBallTest : MonoBehaviour {

	Transform tr;
	Vector2 swipeVector = new Vector2(0,0);
	public float m_speed = .1f;


	void Start () {
		tr = GetComponent<Transform>() as Transform;
	}
	
	void Update () {
		if(SwipeManager.Instance.IsSwiping()) {
			swipeVector = SwipeManager.Instance.GetSwipeVector();
		}
	}

	void FixedUpdate() {
		tr.position = tr.position + swipeVector.x * m_speed * transform.right * Time.fixedDeltaTime + swipeVector.y * m_speed * transform.up * Time.fixedDeltaTime;
	}
}
