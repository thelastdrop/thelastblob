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
    private int layerMaskPlatforms = 1 << 8;

    // Shooting
    private Transform m_shoottr;
    private Transform m_shoottr_right;
    private Transform m_shoottr_left;
    public AudioClip m_shoot_clip;
    public GameObject m_shot_prefab;
    public float shoot_cd = 1f;
    public int shots_count = 4;
    public float shot_interval = 0.3f;
    private float last_use = 0f;
    public int pooledAmount = 5;

    // Animator related variables
    private Animator animator;
    private bool moving = false;
    private bool shooting = false;

    void Start()
    {
        tr = GetComponent<Transform>() as Transform;
        sr = GetComponent<SpriteRenderer>() as SpriteRenderer;
        animator = GetComponent<Animator>() as Animator;
        m_shoottr_right = tr.GetChild(0);
        m_shoottr_left = tr.GetChild(1);

        ObjectPoolingManager.Instance.CreatePool(m_shot_prefab, pooledAmount, 10);
    }

    void Update()
    {
        animator.SetFloat("Speed", m_speed);
        animator.SetBool("Shooting", shooting);
    }

    void FixedUpdate()
    {
        if(!shooting) Move();

        // Shoot player if seen in straight line
        if(Time.time - last_use > shoot_cd)
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(tr.position, m_mov_verse * tr.right);
            if (hits != null)
            {
                foreach (RaycastHit2D hit in hits)
                {
                    // If it hits first a platform don't shoot, layer 8 = Platforms
                    if (hit.collider.gameObject.layer == 8)
                    {
                        shooting = false;
                        break;
                    }
                    
                    if (hit.collider.gameObject.tag == "Player")
                    {                        
                        shooting = true;
                        m_shoottr = !sr.flipX ? m_shoottr_right : m_shoottr_left;                            
                        StartCoroutine(Shoot());
                        break;
                    }
                }
            }
            last_use = Time.time;
        }
    }

    void Idle()
    {
        moving = false;
        m_speed = 0;
    }

    void Move()
    {
        moving = true;
        RaycastHit2D[] hits = Physics2D.RaycastAll(tr.position, -tr.up, raycastMagnitude, layerMaskPlatforms);
        RaycastHit2D[] hitsRight = Physics2D.RaycastAll(tr.position, tr.right, raycastMagnitude, layerMaskPlatforms);
        RaycastHit2D[] hitsLeft = Physics2D.RaycastAll(tr.position, -tr.right, raycastMagnitude, layerMaskPlatforms);

        if(hits.Length < 1 || hitsRight.Length > 0 || hitsLeft.Length > 0)
            Turn();

        // Move
        tr.position = tr.position + m_mov_verse * m_speed * transform.right * Time.fixedDeltaTime;
    }

    void Turn()
    {
        m_mov_verse *= -1;    // Move in other direction
        sr.flipX = m_mov_verse > 0 ? false : true;    // Flip the sprite
    }

    // TODO
    void ShootOne()
    {
        GameObject shot = ObjectPoolingManager.Instance.GetObject(m_shot_prefab.name);//Instantiate(m_shot_prefab, m_shoottr.position, m_shoottr.rotation) as GameObject;
        shot.transform.position = m_shoottr.position;
        shot.transform.rotation = m_shoottr.rotation;
        SoundManager.Instance.PlayModPitch(m_shoot_clip);
    }

    IEnumerator Shoot() {
        yield return new WaitForSeconds(0.5f);
        shooting = true;
        for(int i = 0; i < shots_count; i++)
        {
            ShootOne();
            yield return new WaitForSeconds(shot_interval);
        }
    }
}
