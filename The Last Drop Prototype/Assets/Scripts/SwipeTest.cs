using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeTest : MonoBehaviour {

	Transform tr;
	private Vector2 touchOrigin = -Vector2.one;
	private RaycastHit2D hit;
	private float x = 0f;
	private float y = 0f;
	float speed = .03f;

	// Use this for initialization
	void Start () {
		tr = GetComponent<Transform>() as Transform;
	}
	
	// Update is called once per frame
	void Update () {
		#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE // mobile controls

		if (Input.touchCount > 0) {
        	Touch myTouch = Input.touches[0];
        	
			if (myTouch.phase == TouchPhase.Began) {
            	touchOrigin = myTouch.position;
				hit = Physics2D.Raycast (touchOrigin, tr.position, .1f);
				Debug.Log("inside if");
            } else if (/*hit &&*/ myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0) {
            	Vector2 touchEnd = myTouch.position;                    
                x = touchEnd.x - touchOrigin.x;
                y = touchEnd.y - touchOrigin.y;
				//float dist = Vector2.Distance(touchOrigin,touchEnd);
				Debug.Log("inside else if");
                touchOrigin.x = -1;
            }  
        }

		#endif
	}

	void FixedUpdate() {
		tr.position = tr.position + x * transform.right * speed * Time.fixedDeltaTime + y * transform.up * speed * Time.fixedDeltaTime;
	}
}
