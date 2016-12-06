using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
    Transform tr;
    
    public GameObject m_Particle;

    [Header("Character Movement Stats -- Not in Use")]
    [Tooltip("Character increments of speed H axis")]
    public float m_Speed_H;
    [Tooltip("Character increments of speed V axis")]
    public float m_Speed_V;
    [Tooltip("Character max total speed, counting gravity too")]
    public float m_Max_Speed;

    [Space(5), Header("Character Ability, Stretching")]
    [Tooltip("Cooldown for ability on Fire1 input")]
    public float m_Ability1_CD;
    [Tooltip("Force applied to ability")]
    public float m_Ability1_force;
    [Tooltip("%(0.0 - 1.0) of particles used for stretching"), Range(0f, 1f)]
    public float m_Ability1_Perc_Particle_Used;


    private float m_V_Axis1;
    private float m_H_Axis1;
    private float m_V_Axis2;
    private float m_H_Axis2;

    private Vector2 m_player_applied_speed;

    private float m_last_time_ability1;

    // Use this for initialization
    void Start ()
    {
        tr = gameObject.GetComponent<Transform>();

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

        if ( (      Input.GetButton("Fire1")                   ) &&
             (Time.time - m_last_time_ability1) > m_Ability1_CD) 
        {
                m_last_time_ability1 = Time.time;
                Stretch(new Vector2(1.0f, 1.0f).normalized);
            // Logic of ability one
        }
/*
        if (Input.GetButton("Fire2"))
        {
        }
*/
        //  Debug to test input
        //        Debug.Log("H axis1: " + m_H_Axis1.ToString() + "H axis2: " +  m_H_Axis2.ToString() + "V axis1: " + m_V_Axis1.ToString() + "V axis2: " + m_V_Axis2.ToString());
    }

    void FixedUpdate()
    {
        if ( !GameManager.Instance.m_Gravity_Type )
        {   // gravity changed to the next, or previous index based on H Axis 1, check game manager
            if (m_H_Axis1 != 0.0f)
            {
                GameManager.Instance.Gravity_Change( (m_H_Axis1 > 0) ? true : false );
            }
        }
        else   // continous gravity adjustments
        {
 /*           m_player_applied_speed.Set(0.0f, 0.0f);
            if (m_H_Axis1 != 0.0f) m_player_applied_speed += new Vector2(Physics2D.gravity.y, -Physics2D.gravity.x).normalized * m_Speed_H * m_H_Axis1 * Time.fixedDeltaTime * -1;//force applied perpendiculary to gravity
            if (m_V_Axis1 != 0.0f) m_player_applied_speed += new Vector2(-Physics2D.gravity.x, -Physics2D.gravity.y).normalized * m_Speed_V * m_V_Axis1 * Time.fixedDeltaTime;
            m_player_applied_speed += rb.velocity;
            rb.velocity = (rb.velocity.magnitude > m_Max_Speed) ? (m_player_applied_speed.normalized * m_Max_Speed) : m_player_applied_speed;
            if ((m_H_Axis2 != 0.0f) && (!Input.GetButton("Fire1")))
            {
                Physics2D.gravity = Quaternion.Euler(0f, 0f, m_H_Axis2 * Time.fixedDeltaTime * 100.0f) * Physics2D.gravity;
        }
 */      }
    }
    
    void Stretch( Vector2 direction )
    {
        // Stretching is a line of particles, that are taken from the list and thrown
        float parts_used = (float) GameManager.Instance.m_Player_Avatar_Cs.No_Particles() / m_Ability1_Perc_Particle_Used;
        
        /*
         * Possibilities:
         1) The particles are removed and linked togheder one after the other, made not collide between each others and
            then thrown in a direction, then the player will be naturally be pushed towards the things, and if they overlaps then are absorbed back
         2) The particles are throwed with different forces in the direction of the stretch: easiest way out, but it will make the player going around strangely
            _ maybe i can compensate the impulse added to the particles so that the player can't really move much with it.
         * */

    }
}
