using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cargo : MonoBehaviour, IConsoleIteration {
    Rigidbody2D rb;

    public AudioClip[] m_Destruct_Sound;
    public AudioClip[] m_Hit_Sound;

    [Tooltip("This value is compared with a multiplation of cargo speed and point of impact normal, to get only perpendicular speed")]
    public float m_Breaking_Speed = 1.4f;

    void OnDisable()
    {
    }

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
            float distance = (GameManager.Instance.m_Player.transform.position - gameObject.transform.position).magnitude;
            if (distance < SoundManager.Instance.m_Max_Distance)
                SoundManager.Instance.PlayLevelSound(m_Destruct_Sound[Random.Range(0, m_Destruct_Sound.Length - 1)], true, distance);
            other.gameObject.transform.parent.gameObject.SetActive(false);
		}

        // if other is part of the platforms layer, and the speed magnetude is greater than m_Breaking speed, destroy it!
        if( other.gameObject.layer == LayerMask.NameToLayer("Platforms") )
        {
            foreach (ContactPoint2D elem in other.contacts)
            {
                Vector2 relative_speed = new Vector2(rb.velocity.x * other.contacts[0].normal.x, rb.velocity.y * other.contacts[0].normal.y);
                if (relative_speed.magnitude > m_Breaking_Speed)
                {
                    float distance = (GameManager.Instance.m_Player.transform.position - gameObject.transform.position).magnitude;
                    if (distance < SoundManager.Instance.m_Max_Distance)
                        SoundManager.Instance.PlayLevelSound(m_Destruct_Sound[Random.Range(0, m_Destruct_Sound.Length - 1)], true, distance);
                    //                Debug.Log("Speed: " + rb.velocity.magnitude);
                    gameObject.SetActive(false);
                    break;
                }
                else
                {
                    float distance = (GameManager.Instance.m_Player.transform.position - gameObject.transform.position).magnitude;
                    if (distance < SoundManager.Instance.m_Max_Distance)
                        SoundManager.Instance.PlayLevelSound(m_Hit_Sound[Random.Range(0, m_Hit_Sound.Length - 1)], true, distance);
                }
            }
        }
	}

	public void Activate_Once()
	{
        gameObject.GetComponent<Rigidbody2D>().isKinematic = false;
    }
}
