using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using POLIMIGameCollective;

public class SwipeManager : Singleton<SwipeManager> {

	private Rect left_rect;
	private Rect right_rect;
	private Vector2 touchOrigin = -Vector2.one;
	// Vector representing swipe input
	private Vector2 swipeVector;
	
	void Start() {
		left_rect = new Rect(0f, 0f, Screen.width * 0.5f, Screen.height);
     	right_rect = new Rect(Screen.width * 0.5f, 0f, Screen.width * 0.5f, Screen.height);
	}

	void Update () {
		SwipeDetectionNTrigger();
	}

	// Import this in GameManager
	void SwipeDetectionNTrigger() {
#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE // mobile controls

		// reset value each frame
		swipeVector = Vector2.zero;

		if (Input.touchCount > 0) {
			
			Touch myTouch = Input.touches[0];

        	if(right_rect.Contains(myTouch.position)) {
				if (myTouch.phase == TouchPhase.Began) {
            		touchOrigin = myTouch.position;
            	} else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0) {
            		Vector2 touchEnd = myTouch.position;                    
                	swipeVector = new Vector2(touchEnd.x - touchOrigin.x, touchEnd.y - touchOrigin.y);
					//float dist = Vector2.Distance(touchOrigin,touchEnd);
                	touchOrigin.x = -1;

					// Trigger event: i.e. swipeVector has changed 
					EventManager.TriggerEvent("Swipe");
            	}
			}
        }
#endif
	}

	// Ignore if using TriggerEvent
	public Vector2 GetSwipeVector() {
		return swipeVector;		
	}

	public bool IsSwiping() {
		return swipeVector.magnitude != 0f;
	}

}
