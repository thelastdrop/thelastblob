using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAvatar_02 : MonoBehaviour
{
    Transform tr;
    Rigidbody2D rb;
    //    CircleCollider2D m_Circle_Coll;

    public string m_Layer_Static;
    public string m_Layer_Player;
    public LayerMask m_Layer_Raycast;
    public GameObject m_Particle;

    [Header("Shape Stats"), Tooltip("Number of particles around the center"), Range(8, 100)]
    public int m_No_Particles = 8;
    [Tooltip("Distance of the Raycast at rest, radius of the drop")]
    public float m_Radius = 1.0f;
    [Tooltip("maximum % of change in the gravity axis"), Range(-5f, 5f)]
    public float m_Max_Change = 0.2f;
    [Tooltip("Gravity multiplier, it will multiply the effect of gravity changing the offset of the circle collider"), Range(-5f, 5f)]
    public float m_Gravity_Mult = 0.2f;
    [Tooltip("Acceleration multiplier, it will multiply the effect of acceleration"), Range(-1f, 1f)]
    public float m_Accel_Mult = 0.2f;
    public AnimationCurve m_GravDistance_Curve;

    // List to store values of the verts in the procedural mesh, based on the numbers of raycasts
    // Record [0] store the center of the mesh information.
    private List<RB_vert> m_Vlist = new List<RB_vert>();

    private Vector2 m_old_speed;
    private bool m_isgrounded;

    private Vector2[] m_CosSin;
    float m_Radii_Segment; // radial segment size by number of raycasts

    public struct RB_vert
    {
        public static GameObject Center;
        public GameObject particle;
        public Transform tr;
        public Rigidbody2D rb;
        public SpringJoint2D to_center;
        public SpringJoint2D to_prev;
        
        public RB_vert( GameObject part_ref )
        {
            particle = part_ref;
            tr = particle.GetComponent<Transform>();
            rb = particle.GetComponent<Rigidbody2D>();

            to_prev = null;
            to_center = null;
        }

        public void set_center()
        {
            Center = particle;
        }

        public void center_spring()
        {
            to_center = particle.AddComponent<SpringJoint2D>();
            to_center.connectedBody = Center.GetComponent<Rigidbody2D>();
        }

        public void prev_spring(GameObject prev_ref)
        {
            to_prev = particle.AddComponent<SpringJoint2D>();
            to_prev.connectedBody = prev_ref.GetComponent<Rigidbody2D>();
        }
    }

    // Use this for initialization
    void Start()
    {
        tr = gameObject.GetComponent<Transform>();
        rb = gameObject.GetComponent<Rigidbody2D>();

        //        Physics2D.IgnoreLayerCollision( LayerMask.NameToLayer(m_Layer_Player), LayerMask.NameToLayer(m_Layer_Static));

        calc_cossin(); // Setup everything needed depending on the number of "Raycasts", like CosSin, number of
                       // vertices for the mesh generation ecc

        make_vertex_list(); // actually building the list of vertices used by mesh maker

 
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

    void make_vertex_list()  // First time run to make the vertex list used by procedural mesh and polygon collider path
    {
        m_Vlist.Clear();
        m_Vlist.Add(new RB_vert( Instantiate(m_Particle, tr.position, Quaternion.identity, tr) as GameObject));
        m_Vlist[0].set_center();
        Vector3 position = Vector3.zero;
        

        for (int i = 0; i < m_No_Particles; i++)
        {
            position.Set(m_Radius * m_CosSin[i].x, m_Radius * m_CosSin[i].y, tr.position.z);

            m_Vlist.Add(new RB_vert(Instantiate(m_Particle, tr.position + position,Quaternion.identity, tr) as GameObject));
            m_Vlist[i + 1].center_spring();
            if (i != 0)
            {
                if (i == m_No_Particles - 1)
                {
                    m_Vlist[1].prev_spring(m_Vlist[i + 1].particle);
                } else
                {
                    m_Vlist[i + 1].prev_spring(m_Vlist[i].particle);
                }
            }
        }

        // Setup the Polygon Collider
    }
}
