using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeTest : MonoBehaviour {

	Transform tr;
	private Vector2 touchOrigin = -Vector2.one;
	private Vector2 swipeVector; // Vector representing swipe input
	public float speed = .03f;

	void Start () {
		tr = GetComponent<Transform>() as Transform;
	}
	
	void Update () {
		#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE // mobile controls

		if (Input.touchCount > 0) {
        	Touch myTouch = Input.touches[0];
        	
			if (myTouch.phase == TouchPhase.Began) {
            	touchOrigin = myTouch.position;
            } else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0) {
            	Vector2 touchEnd = myTouch.position;                    
                swipeVector = new Vector2(touchEnd.x - touchOrigin.x, touchEnd.y - touchOrigin.y);
				//float dist = Vector2.Distance(touchOrigin,touchEnd);

                touchOrigin.x = -1;
            }
        }

		#endif
	}

	void FixedUpdate() {
		// Moving a GameObject test
		tr.position = tr.position + swipeVector.x * transform.right * speed * Time.fixedDeltaTime + swipeVector.y * transform.up * speed * Time.fixedDeltaTime;
	}
}
