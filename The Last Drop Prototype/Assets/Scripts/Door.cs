using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IConsoleIteration {

    public GameObject Slab;
    public GameObject Engine;

    [Tooltip("Speed at which the door open")]
    public float m_Time;
    [Tooltip("Distance of the slab to open")]
    public float m_Distance;
    
	public void Activate_Once()
	{
        // Open the fucking door!
    }

    void OpenTheDoor()
    {

    }
}
