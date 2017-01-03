using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cargo : MonoBehaviour, IConsoleIteration {
    Rigidbody2D rb;

    [Tooltip("This value is compared with a multiplation of cargo speed and point of impact normal, to get only perpendicular speed")]
    public float m_Breaking_Speed = 1.4f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

	void OnCollisionEnter2D(Collision2D other)
	{
		if(other.gameObject.tag == "LaserSource")
		{
			// Disable whole parentlaser if Cargo collides with LaserSource
			// other.gameObject = LaserSource GO
			// other.gameObject.transform.parent.gameObject = LaserParent GO
			other.gameObject.transform.parent.gameObject.SetActive(false);
		}

        // if other is part of the platforms layer, and the speed magnetude is greater than m_Breaking speed, destroy it!
        if( other.gameObject.layer == LayerMask.NameToLayer("Platforms") )
        {
            if (other.contacts.Length > 1) Debug.Log("More than one contact registered in Cargocontainer: " + gameObject.name);
            Vector2 relative_speed = new Vector2( rb.velocity.x * other.contacts[0].normal.x, rb.velocity.y * other.contacts[0].normal.y);
            if(relative_speed.magnitude > m_Breaking_Speed)
            {
                Debug.Log("Speed: " + rb.velocity.magnitude);
                gameObject.SetActive(false);
            }
        }
	}

	public void Activate_Once()
	{
        gameObject.GetComponent<Rigidbody2D>().isKinematic = false;
    }
}
