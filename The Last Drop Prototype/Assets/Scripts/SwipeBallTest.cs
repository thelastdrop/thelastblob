using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeBallTest : MonoBehaviour {

	Transform tr;
	Vector2 swipeVector = new Vector2(0,0);


	void Start () {
		tr = GetComponent<Transform>() as Transform;
	}
	
	void Update () {
		if(SwipeManager.Instance.IsSwiping()) {
			swipeVector = SwipeManager.Instance.GetSwipeVector();
		}
	}

	void FixedUpdate() {
		tr.position = tr.position + swipeVector.x * transform.right * Time.fixedDeltaTime + swipeVector.y * transform.up * Time.fixedDeltaTime;
	}
}
