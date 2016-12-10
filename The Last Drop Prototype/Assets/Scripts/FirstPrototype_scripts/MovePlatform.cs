using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlatform : MonoBehaviour
{

	HingeJoint2D joint;

	// Use this for initialization
	void Start ()
	{
		joint = GetComponent<HingeJoint2D> () as HingeJoint2D;
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (joint.limitState == JointLimitState2D.LowerLimit || joint.limitState == JointLimitState2D.UpperLimit) {
			JointMotor2D motor = joint.motor;
			motor.motorSpeed = -motor.motorSpeed;
			joint.motor = motor;
		}
	}
}
