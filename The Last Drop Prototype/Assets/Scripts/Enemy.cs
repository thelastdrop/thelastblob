using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using POLIMIGameCollective;

public class Enemy : MonoBehaviour
{

    // Moving
    [Range(0f, 5f)]
    public float m_speed = 1f;
    private int m_mov_verse = 1; // if 1 moving right, else -1: left
    private Transform tr;
    private SpriteRenderer sr;
    [Range(0.1f,2f)]
    public float raycastMagnitude = 0.5f;
    private Vector2 rcFloorDir; //new Vector2(1f,-1f); diagonal vector
    private Vector2 rcRightDir;
    private Vector2 rcLeftDir;    

    // Shooting
    public AudioClip m_shoot_clip;
    public GameObject m_shot_prefab;

    // Animator related variables
    private Animator animator;
    private bool moving = false;
    private bool shooting = false;

    void Start()
    {
        tr = GetComponent<Transform>() as Transform;
        sr = GetComponent<SpriteRenderer>() as SpriteRenderer;
        animator = GetComponent<Animator>() as Animator;
        rcFloorDir = -tr.up;
        rcRightDir = tr.right;
        rcLeftDir = -rcRightDir;
    }

    void Update()
    {
        animator.SetFloat("Speed", m_speed);
        animator.SetBool("Shooting", shooting);
    }

    void FixedUpdate()
    {
        Move();

        // Shoot player if seen in straight line
        /*
        RaycastHit2D[] hits = Physics2D.RaycastAll(tr.position, m_mov_verse * rcmRightDir);
        if (hits != null)
        {
            foreach (RaycastHit2D hit in hits)
            {
                // If it hits first a platform don't shoot, layer 8 = Platforms
                if (hit.collider.gameObject.layer == 8) break;
                
                if (hit.collider.gameObject.tag == "Player")
                {
                    ShootOne(hit.collider.gameObject);
                    break;
                }
            }
        } */

    }

    void Idle()
    {
        moving = false;
        m_speed = 0;
    }

    void Move()
    {
        moving = true;
        RaycastHit2D[] hits = Physics2D.RaycastAll(tr.position, rcFloorDir, raycastMagnitude);
        RaycastHit2D[] hitsRight = Physics2D.RaycastAll(tr.position, rcRightDir, raycastMagnitude);
        RaycastHit2D[] hitsLeft = Physics2D.RaycastAll(tr.position, rcLeftDir, raycastMagnitude);

        if(hits.Length <= 1 || hitsRight.Length > 1 || hitsLeft.Length > 1) Turn();

        // Move
        tr.position = tr.position + m_mov_verse * m_speed * transform.right * Time.fixedDeltaTime;
    }

    void Turn()
    {
        m_mov_verse *= -1;    // Move in other direction
        sr.flipX = m_mov_verse > 0 ? false : true;    // Flip the sprite
    }

    // TODO
    void ShootOne(GameObject player)
    {
        shooting = true;
        GameObject go = ObjectPoolingManager.Instance.GetObject(m_shot_prefab.name);
        Vector2 direction = player.transform.position - tr.position;

        SoundManager.Instance.PlayModPitch(m_shoot_clip);
    }

    // [TEMP] SetActive(false) if collides with player
    /*
    void OnCollisionEnter2D(Collision2D other)
    {
        //Turn();
        if(other.gameObject.tag == "Player")
        {
            // Play test sound when this dies
            SoundManager.Instance.PlayModPitch(testClip);
        }
    }
    */
}
