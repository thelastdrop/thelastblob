using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using POLIMIGameCollective;

public class TouchControlManager : Singleton<TouchControlManager>
{
    private Rect rightR;
    private float xOriginOffset = 0.33f;
    // [Left portion] Move info 
    public Vector2 moveDirection;
    private float maxMovRange;

    // [Right portion] Vector representing swipe input
    public Vector2 swipeVector;
    private Vector2 touchOrigin;

    public VirtualJoystickMove joystickMove;
    // public VirtualJoystickSwipe joystickSwipe;

    void Start()
    {
        rightR = new Rect(Screen.width * xOriginOffset, 0f, Screen.width * (1 - xOriginOffset), Screen.height);
        maxMovRange = Screen.width * 0.05f;
    }

    void Update()
    {
#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
        moveDirection = Vector2.ClampMagnitude(joystickMove.inputDirection, maxMovRange);
        // swipeVector = joystickSwipe.inputDirection * 10;
        if (Input.touchCount > 0)
        {
            for (int i=0; i < Input.touchCount; i++)
            {
                Touch touch = Input.touches[i];
                
                if(rightR.Contains(touch.position))
                {
                    if (touch.phase == TouchPhase.Began)
                    {
                        touchOrigin = touch.position;
                    }
                    else if (touch.phase == TouchPhase.Ended)
                    {
                        Vector2 touchEnd = touch.position;
                        swipeVector = touchEnd - touchOrigin;

                        // Trigger event: i.e. swipeVector has changed 
                        EventManager.TriggerEvent("Swipe");
                    }
                }
            }
        }        
#endif
    }

    public Vector2 GetSwipeVector()
    {
        return swipeVector;
    }

    public bool IsSwiping()
    {
        return swipeVector.magnitude != 0f;
    }

    private Rect GetWorldRect (RectTransform rt, Vector2 scale)
    {
         // Convert the rectangle to world corners and grab the top left
         Vector3[] corners = new Vector3[4];
         rt.GetWorldCorners(corners);
         Vector3 topLeft = corners[0];
 
         // Rescale the size appropriately based on the current Canvas scale
         Vector2 scaledSize = new Vector2(scale.x * rt.rect.size.x, scale.y * rt.rect.size.y);
 
         return new Rect(topLeft, scaledSize);
    }

}