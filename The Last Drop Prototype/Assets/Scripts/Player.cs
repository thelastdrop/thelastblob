﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {
    Transform tr;
    
//    public GameObject m_Particle;

    [Header("Character Movement Stats -- Not in Use")]
    [Tooltip("Character increments of speed H axis")]
    public float m_Speed_H;
    [Tooltip("Character increments of speed V axis")]
    public float m_Speed_V;
    [Tooltip("Character speed, it will be multiply by the number of particle in contact with layermasks(dynam particle)")]
    public float m_Speed;

    [Space(5), Header("Character Ability, Stretching")]
    public GameObject m_Stretch_Pointer;
    [Tooltip("Using mouse?")]
    public bool m_isUsing_Mouse = false;
    [Tooltip("Cooldown for ability on Fire1 input")]
    public float m_Ability1_CD;
    [Tooltip("Length multiplier")]
    public float m_Ability1_Length = 0.05f;
    [Tooltip("Min Length, after which the elastic will break")]
    public float m_Ability1_Min_Length = 0.1f;
    [Tooltip("Layers the stretching will stick to")]
    public LayerMask m_Ability1_Layers;
    [Tooltip("%(0.0 - 1.0) of particles used for stretching"), Range(0f, 1f)]
    public float m_Ability1_Perc_Particle_Used;
    [Tooltip("Strenght applied by the elastic")]
    public float m_Ability1_Tensile_Str = 1.0f;
    [Tooltip("In how much time the elastic will reach full extend?")]
    public float m_Ability1_Extension_Speed = 0.3f;
    [Tooltip("Restitution force multiplier: How much force will be applied to the other object(if rigid body is present)")]
    public float m_Ability1_Restitution = 0.15f;

    [Header("Feeding Params"), Tooltip("Array of names of objects that the player can eat")]
    public string[] m_Foods = { "Enemy" };
    [Tooltip("How much each food increase particle counts")]
    public int[] m_Nutrition_Value = { 2 };
    public AudioClip[] m_Death_Sound_Eated;

    private float m_V_Axis1;
    private float m_H_Axis1;
    private float m_V_Axis2;
    private float m_H_Axis2;

    // Player Movement
    private bool m_Is_Moving = false;
    private Vector2 m_player_applied_speed;

    // Used by ability1(stretch)
    private int m_Stretch_Condition = 0; // It's Kind of enum, 0 = stretch is not being used, 1 stretch is expanding, 2 stretch is latched to something
    private Vector2 m_Last_Direction;
    private float m_last_time_ability1;
    private LineRenderer m_Line_Renderer;
    private GameObject m_Central_Particle;
    private Rigidbody2D m_Central_Particle_rb;
    private Vector3[] m_Streching_Points;
    private Vector3 m_Screen_Size;

    public struct stretching_data
    {
        static public GameObject relative_position; // GameObject with position relative to the gameobject origin
        public GameObject hit;
        public Rigidbody2D rb;

        public stretching_data( GameObject stretch_to, Vector2 point)
        {
            hit = stretch_to;
            rb = hit.GetComponent<Rigidbody2D>();
            relative_position.transform.parent = hit.transform;
            relative_position.transform.position = new Vector3(point.x, point.y, 0);
 //           Debug.Log("Passato di qui: " + relative_position.transform.position);
        }

        public void initialize()
        {
            relative_position = Instantiate(new GameObject("stretch_hit"));
        }

        public void reset()
        {
            hit = null;
            relative_position.transform.parent = null;
            rb = null;
        }

        public Vector3 get_point()
        {
            return (relative_position.transform.position);
        }
    }

    private stretching_data m_stretch_data;


    //Eating/carry
    private List<carried_items> m_Carried_Items = new List<carried_items>();

    public struct carried_items
    {
        public GameObject item;
        public bool is_food;
        public float time_since_eated;

        public carried_items( GameObject obj_to_store, bool food )
        {
            item = obj_to_store;
            is_food = food;
            if(is_food)
            {
                time_since_eated = Time.time;
            }
            else
            {
                time_since_eated = 0f;
            }
        }
    }
    
    void Awake()
    {
        tr = gameObject.GetComponent<Transform>();
        DontDestroyOnLoad(gameObject);

        Transform cozy_Restart = tr.GetChild(0);
        DontDestroyOnLoad(cozy_Restart.gameObject);

        if (cozy_Restart == null)
        {
            Debug.Log("Player is missing PlayerRestart object");
        } else
        if (cozy_Restart.gameObject.name != "PlayerRestart")
        {
            Debug.Log("Player has a child not named PlayerRestart!");
        }
        else
        {
            cozy_Restart.transform.parent = null; // Detaching the children, so that can be moved indipendently
            GameManager.Instance.m_Player_ReStart_Position = cozy_Restart.gameObject;
        }
    }
    

    // Use this for initialization
    void Start ()
    {
        m_Line_Renderer = gameObject.GetComponent<LineRenderer>();
        if (m_Line_Renderer == null) Debug.Log("Found no line renderer on player!");
        m_Line_Renderer.enabled = false;

        StartCoroutine(set_central_particle()); // Set the central particle later on. Also setup other things as well, it's kind of a late start routine

        m_Screen_Size = new Vector3( Screen.width, Screen.height, 0f) / 2;
        m_stretch_data.initialize();

        // Triggers Events registration
        POLIMIGameCollective.EventManager.StartListening("Swipe", swipe);
        POLIMIGameCollective.EventManager.StartListening("MoveStart", MoveStart);
        POLIMIGameCollective.EventManager.StartListening("MoveEnd", MoveEnd);
        POLIMIGameCollective.EventManager.StartListening("Shake", Shake);
//      POLIMIGameCollective.EventManager.StartListening("LoadLevel", PlayerReset);
    }

    // Update is called once per frame
    void Update() {
        m_V_Axis1 = Input.GetAxis("Vertical");
        m_H_Axis1 = Input.GetAxis("Horizontal");
        m_V_Axis2 = Input.GetAxis("Vertical2");
        m_H_Axis2 = Input.GetAxis("Horizontal2");

        /********************************************/
        /*    Ability(jump, shoot, stretch ecc)     */
        /********************************************/

        if (Input.GetKeyDown("escape")) GameWinManager.Instance.PauseLevel();


#if UNITY_EDITOR || UNITY_STANDALONE

        Vector2 axis2 = new Vector2( m_H_Axis2, m_V_Axis2 );

        // If mouse is in use, or else the controller is in use
        if (m_isUsing_Mouse == true)
        {
            if ((Input.GetButton("Fire1")) &&
                 (Time.time - m_last_time_ability1) > m_Ability1_CD)
            {
                Stop_Strectching();
                m_Stretch_Condition = 0;
                m_last_time_ability1 = Time.time;
                Vector3 cozy = Input.mousePosition - m_Screen_Size;
                m_Last_Direction = new Vector2(cozy.x, cozy.y).normalized;
                PC_Swipe(m_Last_Direction);

                m_last_time_ability1 = Time.time;
                // Logic of ability one
            }
        }
        else
        {
            if ( axis2.magnitude > 0.5  &&
               ( Time.time - m_last_time_ability1) > m_Ability1_CD )
            {
                //Debug.Log("Shoot!");

                Stop_Strectching();
                m_Stretch_Condition = 0;
                m_last_time_ability1 = Time.time;
                m_Last_Direction = new Vector2(m_H_Axis2, m_V_Axis2).normalized;
                PC_Swipe( m_Last_Direction );
            }

        }

        if (Input.GetButton("Fire2"))
        {
            POLIMIGameCollective.EventManager.TriggerEvent("Shake");
        }
#endif
        //  Debug to test input
        //        Debug.Log("H axis1: " + m_H_Axis1.ToString() + "H axis2: " +  m_H_Axis2.ToString() + "V axis1: " + m_V_Axis1.ToString() + "V axis2: " + m_V_Axis2.ToString());
    }

    void FixedUpdate()
    {
        /*
        if ( !GameManager.Instance.m_Gravity_Type )
        {   // gravity changed to the next, or previous index based on H Axis 1, check game manager
            if (m_H_Axis1 != 0.0f)
            {
                GameManager.Instance.Gravity_Change( ( m_H_Axis1 > 0 ) ? true : false );
            }
        }
        else   // continous gravity adjustments
        {
            m_player_applied_speed.Set(0.0f, 0.0f);
            if (m_H_Axis1 != 0.0f) m_player_applied_speed += new Vector2(Physics2D.gravity.y, -Physics2D.gravity.x).normalized * m_Speed_H * m_H_Axis1 * Time.fixedDeltaTime * -1;//force applied perpendiculary to gravity
            if (m_V_Axis1 != 0.0f) m_player_applied_speed += new Vector2(-Physics2D.gravity.x, -Physics2D.gravity.y).normalized * m_Speed_V * m_V_Axis1 * Time.fixedDeltaTime;
            m_player_applied_speed += rb.velocity;
            rb.velocity = (rb.velocity.magnitude > m_Max_Speed) ? (m_player_applied_speed.normalized * m_Max_Speed) : m_player_applied_speed;
            if ((m_H_Axis2 != 0.0f) && (!Input.GetButton("Fire1")))
            {
                Physics2D.gravity = Quaternion.Euler(0f, 0f, m_H_Axis2 * Time.fixedDeltaTime * 100.0f) * Physics2D.gravity;
            }
        }
        */
        
        // PC / TEST LOGIC WITH KEYBOARD OF GAMEPAD
#if UNITY_EDITOR || UNITY_STANDALONE
        if ( (m_H_Axis1 != 0) ||
            (m_V_Axis1 != 0)    )
        {
//            Debug.Log(m_H_Axis1 + " " + m_V_Axis1);
            Vector2 direction = GameManager.Instance.Rotate_By_Gravity( new Vector2(m_H_Axis1, m_V_Axis1) );
            GameManager.Instance.m_Player_Avatar_Cs.AddSpeed(direction * Time.fixedDeltaTime * m_Speed);
        }

#endif

        if (m_Is_Moving)  // Is moving!
        {
            Vector2 direction = GameManager.Instance.Rotate_By_Gravity( TouchControlManager.Instance.moveDirection ); // change rotation by gravity
            GameManager.Instance.m_Player_Avatar_Cs.AddSpeed( direction * Time.fixedDeltaTime * m_Speed);
        }

        // Strecthing logic, if there is a stretch in action(which is true if the Line Render is enabled)
        if( (m_Line_Renderer.enabled)  &&
            (m_Stretch_Condition == 2)   ) // Condition 2 = the stretch is latched
        {
            Vector2 velocity = ((new Vector2(m_Streching_Points[0].x, m_Streching_Points[0].y) - new Vector2(m_Streching_Points[1].x, m_Streching_Points[1].y)) * m_Ability1_Tensile_Str);
            if(m_stretch_data.rb != null) m_stretch_data.rb.AddForceAtPosition( -velocity * (float) GameManager.Instance.m_Player_Avatar_Cs.No_Particles() * m_Ability1_Restitution, m_stretch_data.get_point());
            m_Central_Particle_rb.velocity = (m_stretch_data.rb == null) ? m_Central_Particle_rb.velocity + velocity : m_Central_Particle_rb.velocity + velocity * m_stretch_data.rb.mass * m_Ability1_Restitution;

            Set_Points();
            // Current maximum lenght is: 1.0f * parts_used* m_Ability1_Length

            m_Line_Renderer.enabled = Check_Stretch_Length();
        }

        if( m_Stretch_Condition == 1 )
        {
            PC_Swipe(m_Last_Direction);
        }

        for(int i = 0; i < m_Carried_Items.Count; i++)
        {
            
            if( m_Carried_Items[i].is_food == true )
            {
                if (Time.time - m_Carried_Items[i].time_since_eated >= 1.0f)
                {
                    if(m_Carried_Items[i].item == null)
                    {
                        m_Carried_Items.RemoveAt(i);
                        continue;
                    }
                    m_Carried_Items[i].item.SetActive(false);
                    GameManager.Instance.m_Player_Avatar_Cs.Grow(2);
                    m_Carried_Items.RemoveAt(i);                   
                }
                else
                {
                    m_Carried_Items[i].item.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, Time.time - m_Carried_Items[i].time_since_eated);
                }
            }
        }
    }

    /************************************/
    /***     Trigger Events methods   ***/
    /************************************/
    void swipe()
    {
        m_Stretch_Condition = 0;
        Stop_Strectching();
        m_last_time_ability1 = Time.time;
        m_Last_Direction = TouchControlManager.Instance.GetSwipeVector().normalized;
        PC_Swipe(m_Last_Direction);
//      Stretch( TouchControlManager.Instance.GetSwipeVector().normalized );
    }

    void MoveStart()
    {
        m_Is_Moving = true;
    }

    void MoveEnd()
    {
        m_Is_Moving = false;
    }

    void Shake()
    {
        //        GameManager.Instance.m_Debug_Text.text = " Accel var: " + Shake_Manager.Instance.m_Unbiased_Accel.z + " " + Shake_Manager.Instance.m_Shake_Min_Accel;
        Stop_Strectching();
        //        GameManager.Instance.m_Player_Avatar_Cs.PlayerReset();
    }

    void PlayerReset()
    {
        StartCoroutine( set_central_particle() );
    }
    /*********************************/
    /****    Internal Methods     ****/
    /*********************************/

    IEnumerator set_central_particle()
    {
        yield return new WaitForSeconds(.05f);
        m_Central_Particle = GameManager.Instance.m_Central_Particle;
        m_Central_Particle_rb = m_Central_Particle.GetComponent<Rigidbody2D>();

        m_Stretch_Pointer = GameObject.Find("tentacle_pointer");
    }

    Vector2 Stretch( Vector2 direction )
    {
//        GameManager.Instance.m_Debug_Text.text = "Swipe: " + direction;
        Vector3 direction3 = direction;
        direction = GameManager.Instance.Rotate_By_Gravity(direction);

        float parts_used = (float) GameManager.Instance.m_Player_Avatar_Cs.No_Particles() / m_Ability1_Perc_Particle_Used;

        //        Debug.Log("Position: " + (new Vector2(tr.position.x, tr.position.y) + (direction * parts_used * m_Ability1_Length)) );
        Vector2 end_ray = new Vector2(tr.position.x, tr.position.y) + (direction * parts_used * m_Ability1_Length);
        RaycastHit2D[] hits = Physics2D.LinecastAll(tr.position, end_ray );
        //        Debug.Log( "Hits:" + hits.Length );

        bool hit_register = false;

        foreach( RaycastHit2D elem in hits )
        {
            if (m_Ability1_Layers == (m_Ability1_Layers | (1 << elem.collider.gameObject.layer))) // Is the gameobject layer inside the layermask?
            {
                m_Line_Renderer.enabled = true;
                GameManager.Instance.m_Player_IsStretching = true;
                Set_Points( elem.point );
                m_Stretch_Condition = 2;
                hit_register = true;
                m_stretch_data = new stretching_data( elem.collider.gameObject, elem.point );
//                stretch
                break;
            }
        }

        if(hit_register == false)
        {
            m_Line_Renderer.enabled = false;
        }
        else
        {
            m_Line_Renderer.enabled = Check_Stretch_Length();
        }


        /*
         * Possibilities:
         0) Shit solution: Line render to show blue from one point to the other, and then forcing the central particle toward that point
         1) The particles are removed and linked togheder one after the other, made not collide between each others and
            then thrown in a direction, then the player will be naturally be pushed towards the things, and if they overlaps then are absorbed back
         2) The particles are throwed with different forces in the direction of the stretch: easiest way out, but it will make the player going around strangely
            _ maybe i can compensate the impulse added to the particles so that the player can't really move much with it.
         * */

        return end_ray;
    }

    bool Check_Stretch_Length()
    {
        float lenght = Vector3.Magnitude(m_Streching_Points[0] - m_Streching_Points[1]);
        if ((lenght > (float)GameManager.Instance.m_Player_Avatar_Cs.No_Particles() / m_Ability1_Perc_Particle_Used * m_Ability1_Length) ||
            (lenght < m_Ability1_Min_Length))
        {
            Stop_Strectching();
            return false;
        }
        return true;
    }

    void Set_Points(Vector3 distant_point) // Set the points when the stretch is not latched
    {
        m_Streching_Points = new Vector3[] { new Vector3(distant_point.x, distant_point.y, 0f), new Vector3(tr.position.x, tr.position.y, 0.0f) };
        m_Line_Renderer.SetPositions(m_Streching_Points);
    }

    void Set_Points() // Set the points when the position is latched
    {
        m_Streching_Points[1] = tr.position;
        m_Streching_Points[0] = m_stretch_data.get_point();
        m_Line_Renderer.SetPositions(m_Streching_Points);
    }

    /************************************/
    /***    PUBLIC METHODS            ***/
    /************************************/

    public void Stop_Strectching()
    {
        m_Line_Renderer.enabled = false;
        GameManager.Instance.m_Player_IsStretching = false;
        m_Stretch_Condition = 0;
        m_stretch_data.reset();
    }

    public void PC_Swipe( Vector2 direction )
    {
        //  Debug.Log("Stretching direction: " + direction);
        float lerp_time = Mathf.Clamp( ( (Time.time - m_last_time_ability1) / m_Ability1_Extension_Speed), 0f, 1f);
        //Debug.Log( lerp_time );
        Vector2 line_end_point = Stretch( Vector2.Lerp( new Vector2(0,0), direction, lerp_time ) );
        if( (lerp_time          == 1f) ||
            (m_Stretch_Condition == 2)   )
        {
            m_Stretch_Condition = 2;
            m_Last_Direction = Vector2.zero;
        }
        else
        {
            m_Stretch_Condition = 1;
            m_Line_Renderer.enabled = true;
            Set_Points(line_end_point);
        }
    }

    public void Eat_Carry( GameObject object_carried )
    {
        m_Carried_Items.Add(new carried_items(object_carried, true));
        SoundManager.Instance.PlayDeathSound( m_Death_Sound_Eated[Random.Range(0, m_Death_Sound_Eated.Length - 1)]);
    }
}
