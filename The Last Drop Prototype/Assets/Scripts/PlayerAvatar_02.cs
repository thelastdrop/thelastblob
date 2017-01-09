﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAvatar_02 : MonoBehaviour, ITeleport
{
    Transform tr;
//    CircleCollider2D m_Circle_Coll; // Collider used for triggering player event(consoles for example)

    public string m_Layer_Static;
    public string m_Layer_Player;
    public LayerMask m_Layer_Raycast;
    public GameObject m_Particle;

    [Header("Starting Position object"), Tooltip("if null it will start from transform position of player")]
    public GameObject m_Start_Position_Object;

    [Header("Shape Stats"), Tooltip("Number of particles around the center"), Range(8, 100)]
    public int m_No_Particles = 8;
    [Tooltip("Ideal radius of the drop")]
    public float m_Radius = 1.0f;
    [Tooltip("Maximum number of particles, after which the blob will not grow")]
    public float m_Max_Particles = 30;
    [Tooltip("The avatar will surrender to death after his particle count drop under this")]
    public int m_No_PArticle_Death = 5;

    [Tooltip("Strenght of the bounds toward center")]
    public float m_Center_Bound_Freq;
    [Tooltip("Strenght of the surface buond")]
    public float m_Surface_Buond;

    [Header("Iteractions with other scripts")]
    [Tooltip("Time must pass between a teleport and the other in secs")]
    public float m_Min_Time_ToTeleport = 0.2f;

    [Space(5),Header("Movement")]
    [Tooltip("Air control streght, then is multiplied with the number of particles")]
    public float m_Air_Control = 1.0f;
    [Tooltip("Check if a particle is in contact with the floor every this seconds"), Range(0.008f, 0.1f)]
    public float m_CheckForContact_Repeat_Time = 0.008f;
    [Tooltip("Curve multipling speed from 0(dimension 0) to 1 depending on a ratio between dimensions and max dimensions")]
    public AnimationCurve m_Mass_To_Speed;
    [Tooltip("Curve to correct viscosity depending of ratio between dimensions and max dimension, it's value is negated so time 0 rapresent maximum dimensions and 1 smallest")]
    public AnimationCurve m_Mass_To_Viscosity;
    [Tooltip("Antrigravity: % of inverted gravity vector added to the blob any time it moves, to help move vertically")]
    public float m_Antrigrav = 0.52f;


    // List to store values of the verts in the procedural mesh, based on the numbers of raycasts
    // Record [0] store the center of the mesh information.
    private List<RB_vert> m_Vlist = new List<RB_vert>();
    
    private float m_Last_Teleport;

    /// <summary>
    /// m_Num_In_Contact tell us how many particle are "sticked" to a surface, it should be used to move around the blob
    /// It's speed must be proportional to the number of particle in contact
    /// </summary>
    public int m_Num_In_Contact;
    private int m_TotalParticles; //Used by checkforcontact, to get a rough(not real time!!!) estimate on how big is the blob
    private Vector3 m_Start_Position;

    private Vector2[] m_CosSin;
    float m_Radii_Segment; 


    public struct RB_vert
    {
        public static GameObject Center;
        public GameObject particle;
        public Dynam_Particle particle_script;
        public Transform tr;
        public Rigidbody2D rb;
        public SpringJoint2D to_center;
//      public SpringJoint2D to_prev;
        
        public RB_vert( GameObject part_ref, Vector3 arg_position, Quaternion arg_rotation)
        {
            part_ref.tag = "Player";
            particle = part_ref;

            part_ref.transform.position = arg_position;
            part_ref.transform.rotation = arg_rotation;

            tr = particle.GetComponent<Transform>();
            rb = particle.GetComponent<Rigidbody2D>();

            particle_script = particle.GetComponent<Dynam_Particle>();
            particle_script.m_IsSticky = true;

 //         to_prev = null;
            to_center = null;
        }

        public void set_center()
        {
            Center = particle;
            particle_script.m_IsSticky = false;
        }

        public void center_spring( float freq )
        {
            to_center = particle.AddComponent<SpringJoint2D>();
            to_center.enableCollision = false;
            to_center.connectedBody = Center.GetComponent<Rigidbody2D>();
            to_center.frequency = freq;
        }
/*
        public void prev_spring(GameObject prev_ref)
        {
            to_prev = particle.AddComponent<SpringJoint2D>();
            to_prev.connectedBody = prev_ref.GetComponent<Rigidbody2D>();
        }
*/
        public Vector3 get_center_position()
        {
            return Center.transform.position;
        }

        public void set_bound_tocenter(float str)
        {
            //            to_center.frequency = str;
            //            if (to_center == null) Debug.Log("Cacchiarola");
            SpringJoint2D[] joints = particle.GetComponents<SpringJoint2D>();
            joints[0].frequency = str;
        }

        public void set_bound_surface(float str)
        {
            SpringJoint2D[] joints = particle.GetComponents<SpringJoint2D>();
            joints[1].frequency = str;
        }

        public void set_location( Vector3 arg_location)
        {
            particle.transform.position =  arg_location;
        }
    }

    void OnValidate()
    {

        if (m_Vlist.Count == 0) return;
        Debug.Log(m_Vlist.Count);
        Evaluate_Bounding();
 //     Set_Surface_Buond();

    }

    // Use this for initialization
    void Start()
    {
        tr = gameObject.GetComponent<Transform>();

        m_Start_Position_Object = GameObject.Find("PlayerStart");
        if (m_Start_Position_Object != null)
        {
            m_Start_Position = m_Start_Position_Object.transform.position;
            tr.position = m_Start_Position;
        }
        else
        {
            m_Start_Position = tr.position;
        }

        //        m_Circle_Coll = gameObject.GetComponent<CircleCollider2D>();
        //        Physics2D.IgnoreLayerCollision( LayerMask.NameToLayer("Player_Avatar"), LayerMask.NameToLayer("Metaballs"));

        calc_cossin(); // Setup everything needed depending on the number of "Raycasts", like CosSin, number of
                       // vertices for the mesh generation ecc

        make_vertex_list(); // actually building the list of vertices used by mesh maker



        GameManager.Instance.m_Central_Particle = Get_Central_Particle(); // used to force the movement of the player



/*      POLIMIGameCollective.EventManager.StartListening("PlayerReset", PlayerReset);

        POLIMIGameCollective.EventManager.StartListening("EndLevel", PlayerDestroy);*/
//      POLIMIGameCollective.EventManager.StartListening("LoadLevel", PlayerReset);

/*
        POLIMIGameCollective.EventManager.StartListening("PauseLevel", PlayerDestroy);
        POLIMIGameCollective.EventManager.StartListening("ResumeLevel", PlayerReset);
*/

        /*
                InvokeRepeating( "Check_For_Contact", m_CheckForContact_Repeat_Time, m_CheckForContact_Repeat_Time);
                Debug.Log("Enable");
        */
    }

    void Update()
    {
//        tr.position = m_Vlist[0].get_center_position();
        Check_Median_Point();
    }

    void OnEnable()
    {
        InvokeRepeating("Check_For_Contact", m_CheckForContact_Repeat_Time, m_CheckForContact_Repeat_Time);
        Debug.Log("Enable");
    }

    void OnDisable()
    {
        CancelInvoke( "Check_For_Contact" );
//        Debug.Log("Disable");
    }

    void OnApplicationQuit()
    {
        CancelInvoke("Check_For_Contact");
    }
    /************************************/
    /***    Invoke and coroutines     ***/
    /************************************/
    void Check_For_Contact()
    {
        m_Num_In_Contact = 0;
        m_TotalParticles = m_Vlist.Count;
        for (int i = 1; i < m_TotalParticles; i++)
        {
            if( m_Vlist[i].particle_script.m_Is_InContact_With_Floor )
            {
                m_Num_In_Contact += 1;
            }
        }
        //Debug.Log("Num in contacts:" + m_Num_In_Contact);
    }

    void Check_Median_Point()
    {
        Vector3 median_pos = new Vector3();
        for(int i = 0; i < m_Vlist.Count; i++)
        {
            median_pos += m_Vlist[i].particle.transform.position;
        }

        tr.position = median_pos / m_Vlist.Count;
    }

    /************************************/
    /******** Internal methods **********/
    /************************************/

    void calc_cossin() // Pre Computer Sincos based on the number of raycasts and set
                       // radii_segment, number of vertices ecc.
    {
        m_CosSin = new Vector2[m_No_Particles];
        m_Radii_Segment = (Mathf.PI * 2) / m_No_Particles;

        for (int i = 0; i < m_No_Particles; i++)
        {
            m_CosSin[i] = new Vector2(Mathf.Cos((i + 1) * m_Radii_Segment), Mathf.Sin((i + 1) * m_Radii_Segment)); //+1 because center is not calculated with cossin
        }

    }

    void make_vertex_list()
    {
        m_Vlist.Add(new RB_vert(POLIMIGameCollective.ObjectPoolingManager.Instance.GetObject(m_Particle.name), tr.position, Quaternion.identity));
        m_Vlist[0].set_center();
        m_Vlist[0].particle.GetComponent<CircleCollider2D>().enabled = false;
        m_Vlist[0].particle.GetComponent<SpriteRenderer>().enabled = false;
        //      m_Vlist[0].particle.GetComponentInChildren<Sprite>

        Vector3 position = Vector3.zero;
        

        for (int i = 0; i < m_No_Particles; i++)
        {
            position.Set(m_Radius * m_CosSin[i].x, m_Radius * m_CosSin[i].y, tr.position.z);
            m_Vlist.Add(new RB_vert(POLIMIGameCollective.ObjectPoolingManager.Instance.GetObject(m_Particle.name), tr.position + position, Quaternion.identity));
            m_Vlist[i + 1].center_spring( m_Center_Bound_Freq );

            // Surface bound removed, it's not really usefull
       /*   if (i != 0)
            {
                if (i == m_No_Particles - 1)
                {
                    m_Vlist[1].prev_spring(m_Vlist[i + 1].particle);
                    m_Vlist[i + 1].prev_spring(m_Vlist[i].particle);
                } else
                {
                    m_Vlist[i + 1].prev_spring(m_Vlist[i].particle);
                }
            }*/
        }
        Evaluate_Bounding();
    }

    void Evaluate_Bounding()
    {
        float bound_freq = m_Mass_To_Viscosity.Evaluate( Mathf.Clamp((1f - (m_Vlist.Count / m_Max_Particles)), 0, 1 ));
        Set_Buond_To_Center( bound_freq * m_Center_Bound_Freq);
//        Debug.Log( bound_freq * m_Center_Bound_Freq );
    }

    /****************************/
    /****  PUBLIC METHODS *******/
    /****************************/

    public void Set_Buond_To_Center( float freq )
    {
        for(int i = 1; i < m_Vlist.Count; i++) //start from 1, skipping center
        {
            //Debug.Log(i);
            m_Vlist[i].set_bound_tocenter( freq );
        }
    }

    // Remove specific particle
    public void Deactivate_Particle( GameObject particle )
    {
        for( int i = 0; i < m_Vlist.Count; i++ )
        {
            if(m_Vlist[i].particle == particle)
            {
                if( i == 0 ) // 0 is the center, it must be preserved until everything else is destroyed
                {

                } else
                {
                    m_Vlist.RemoveAt(i);
                    particle.SetActive(false);
                    break;
                }
            }
        }
        SoundManager.Instance.PlayBlobDamage();
        Check_For_Death();
    }

    // Remove N particles at random
    public void Deactivate_Particle( int no_particles )
    {
        for (int i = 0; i < no_particles; i++)
        {
            int random_element = Random.Range(1, m_Vlist.Count - 1 );
            m_Vlist[random_element].particle.SetActive(false);
            m_Vlist.RemoveAt(random_element);
            Check_For_Death();
        }
    }

    // Check to see if the player has to die, in case call the method from gamewinmanager
    public bool Check_For_Death()
    {
        if (m_Vlist.Count < m_No_PArticle_Death)
        {
            GameWinManager.Instance.LoseLevel();
            return true;
        }
        return false;
    }

    // Return a random particle reference save from the center
    public GameObject Get_Rand_Particle()
    {
        return m_Vlist[ Random.Range(1, m_Vlist.Count - 1) ].particle;
    }

    // Return number of particles
    public int No_Particles()
    {
        return m_Vlist.Count;
    }

    public GameObject Get_Central_Particle()
    {
        return m_Vlist[0].particle;
    }

    public void AddSpeed( Vector2 Speed )
    {
        float multiplier = m_Mass_To_Speed.Evaluate(Mathf.Clamp(((m_Vlist.Count / m_Max_Particles)), 0, 1));

        if (GameManager.Instance.m_Player_IsStretching == false)
        {
            m_Vlist[0].rb.AddForce( ( Speed * m_Num_In_Contact ) * multiplier);
        }
        else
        {
            m_Vlist[0].rb.AddForce( ( (Speed * m_Num_In_Contact) + ( Speed.normalized * m_Air_Control * m_Vlist.Count ) ) * multiplier);
        }
        // Anti-Gravity
        if(m_Num_In_Contact > 0) m_Vlist[0].rb.AddForce(-Physics2D.gravity * m_Antrigrav * m_Vlist.Count );
//        Debug.Log("Speed: " + (Speed * m_Num_In_Contact) );
    }

    public void Grow( int no_particles )
    {
//        Debug.Log("Add particle: " + no_particles);
        Vector3 position = Vector3.zero;
        for (int i = 0; i < no_particles; i++)
        {
            if( m_Vlist.Count >= m_Max_Particles )
            {
                break;
            }
            int rand_sincos_ind = Random.Range( 0, m_No_Particles );

            position.Set(m_Radius * m_CosSin[rand_sincos_ind].x, m_Radius * m_CosSin[rand_sincos_ind].y, tr.position.z);
            m_Vlist.Add(new RB_vert(POLIMIGameCollective.ObjectPoolingManager.Instance.GetObject(m_Particle.name), tr.position + position, Quaternion.identity));
            m_Vlist[m_Vlist.Count - 1].center_spring(m_Center_Bound_Freq);
        }
        Evaluate_Bounding();
    }
    /*
        public void Set_Surface_Buond()
        {
            for (int i = 1; i < m_No_Particles + 1; i++) //start from 1, skipping center
            {
                Debug.Log(i);
                m_Vlist[i].set_bound_surface( m_Surface_Buond );
            }
        }
    */

    /***************************************/
    /********* TRIGGER EVENTS **************/
    /***************************************/
    public void PlayerReset( int particles )
    {
        for (int i = 0; i < m_Vlist.Count; i++)
        {
            m_Vlist[i].particle.SetActive(false);
        }

        m_Vlist.Clear();

        m_Start_Position_Object = GameObject.Find("PlayerStart");
        if (m_Start_Position_Object != null)
        {
            m_Start_Position = m_Start_Position_Object.transform.position;
        }
        tr.position = m_Start_Position;

        m_No_Particles = particles;

        calc_cossin(); 
        make_vertex_list(); 
        GameManager.Instance.m_Central_Particle = Get_Central_Particle();
        CameraManager.Instance.Reset_To_Start();
        GameManager.Instance.Gravity_Reset();

 //       Set_Buond_To_Center(m_Center_Bound_Freq);
        //      Debug.Log("Reset!");
    }

    public void PlayerDestroy()
    {

        for (int i = 0; i < m_Vlist.Count; i++)
        {
            m_Vlist[i].particle.SetActive(false);
        }
        m_Vlist.Clear();
    }

    public void PlayerRemake()
    { 
        m_Start_Position_Object = GameObject.Find("PlayerStart");
        if (m_Start_Position_Object != null)
        {
            m_Start_Position = m_Start_Position_Object.transform.position;
        }

        tr.position = m_Start_Position;

        calc_cossin();
        make_vertex_list();
        GameManager.Instance.m_Central_Particle = Get_Central_Particle();
    }

    /***************************************/
    /*********    INTERFACES ***************/
    /***************************************/

    public void Teleport_To(Vector3 relative_position, Vector3 direction)
    {
        if( Time.time - m_Last_Teleport >= m_Min_Time_ToTeleport )
        {
            for (int i = 0; i < m_Vlist.Count; i++)
            {
                m_Vlist[i].set_location(relative_position);
                float speed = m_Vlist[i].rb.velocity.magnitude;
                m_Vlist[i].rb.velocity = direction * speed;
            }
            m_Last_Teleport = Time.time;
        }
        
    }
}
