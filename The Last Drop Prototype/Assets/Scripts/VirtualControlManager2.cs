using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using POLIMIGameCollective;

public class VirtualControlManager2 : Singleton<VirtualControlManager2>
{
    // [Left portion] Move info 
    public Vector2 moveDirection;
    private float maxMovRange;

    // [Right portion] Vector representing swipe input
    public Vector2 swipeVector;

    public VirtualJoystickMove joystickMove;
    public VirtualJoystickSwipe joystickSwipe;

    void Start()
    {
        maxMovRange = Screen.width * 0.05f;
    }

    void Update()
    {
#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
        moveDirection = Vector2.ClampMagnitude(joystickMove.inputDirection, maxMovRange);
        swipeVector = joystickSwipe.inputDirection * 10;
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

}