using UnityEngine;
using System.Collections;

public class Dynam_Particle : MonoBehaviour
{

    public Rigidbody2D rb;

    [Tooltip("How much force the particle use vs the elements it collide with")]
    public float m_Sticknes = 0.5f;
    [Tooltip("Which layers the drop collide with")]
    public LayerMask m_Stick_To_Layers;

    public bool m_IsSticky = false;
    public bool m_Is_InContact_With_Floor = false;

    void start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        //      startTime = Time.time;
        m_Is_InContact_With_Floor = false;
        //    StartCoroutine(is_sticky());
    }

    void OnDisable()  // Reset state of the particle so that can be placed back to the pool
    {
        
        SpringJoint2D[] joints = gameObject.GetComponents<SpringJoint2D>();
        foreach( SpringJoint2D elem in joints )
        {
            Destroy(elem);
        }
        m_IsSticky = false;
        m_Is_InContact_With_Floor = false;

    }
    
    // If the particle is Sticky and hit something in the sticky layer mask, it will adhere with some force
    void OnCollisionEnter2D(Collision2D other)
    {
        if ( (other.gameObject.layer == LayerMask.NameToLayer("Enemy_Food")) &&
            (gameObject.tag         == "Player") &&
            (other.gameObject.GetComponent<Enemy>().enabled)                     )
        {
            Rigidbody2D otherRb = other.gameObject.GetComponent<Rigidbody2D>();
            otherRb.isKinematic = false;
            other.gameObject.GetComponent<Enemy>().enabled = false;
            other.gameObject.GetComponent<Animator>().enabled = false;
            GameManager.Instance.m_Player.GetComponent<Player>().Eat_Carry(other.gameObject);
//            Debug.Log("Enemy eated");
        }

        // Stickness, anything not related to it before this line!
        if (!m_IsSticky) return; // return if not sticky!

        foreach (ContactPoint2D elem in other.contacts)
        {
            if (m_Stick_To_Layers == (m_Stick_To_Layers | ( 1 << elem.collider.gameObject.layer )))
            {
                rb.velocity = rb.velocity + (-elem.normal * m_Sticknes);
                m_Is_InContact_With_Floor = true;
            }
        }
    }

    void OnCollisionStay2D(Collision2D other)
    {
        // Stickness, anything not related to it before this line!
        if (!m_IsSticky) return; // return if not sticky!

        foreach (ContactPoint2D elem in other.contacts)
        {
            if (m_Stick_To_Layers == (m_Stick_To_Layers | (1 << elem.collider.gameObject.layer)))
            {
              //  Debug.Log("Normal: " + elem.normal);

                rb.velocity = rb.velocity + (-elem.normal * m_Sticknes);
            }
        }

    }

    void OnCollisionExit2D(Collision2D collision)
    {
        foreach (ContactPoint2D elem in collision.contacts)
        {
            if (m_Stick_To_Layers == (m_Stick_To_Layers | (1 << elem.collider.gameObject.layer)))
                m_Is_InContact_With_Floor = false;
        }
    }

    IEnumerator is_sticky()
    {
        yield return new WaitForSeconds(.05f);
        if(gameObject.tag == "Player")
        {
            m_IsSticky = true;
        }
    }


    public Rigidbody2D get_rb()
    {
        return rb;
    }

}