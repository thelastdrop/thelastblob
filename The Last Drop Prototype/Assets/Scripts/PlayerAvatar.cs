using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAvatar : MonoBehaviour {
    Transform tr;
    Rigidbody2D rb;
    PolygonCollider2D coll;


    [Header("Shape Stats"), Tooltip("Number of raycast shoot from the center"), Range(8,100)]
    public int m_Raycasts = 8;
    [Tooltip("Distance of the Raycast at rest, radius of the drop")]
    public float m_Radius = 1.0f;
    [Tooltip("Gravity multiplier, it will multiply the effect of gravity"), Range(-1f, 1f)]
    public float m_Gravity_Mult = 0.2f;
    [Tooltip("Acceleration multiplier, it will multiply the effect of acceleration"), Range(-1f, 1f)]
    public float m_Accel_Mult = 0.2f;
    

    private List<RB_vert> m_Vlist = new List<RB_vert>();
    private int m_Drop_Vertices;
    private Vector2 m_old_speed;

    private Vector2[] m_CosSin;

    struct RB_vert
    {
        public Vector3 position;

        public RB_vert( Vector2 local_loc )
        {
            position = local_loc;
        }
    }

	// Use this for initialization
	void Start () {
        tr = gameObject.GetComponent<Transform>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        coll = gameObject.GetComponent<PolygonCollider2D>();


        calc_sincos();
        ray_cast();
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    /************************************/
    /******** Internal methods **********/
    /************************************/
    void calc_sincos() // Pre Computer Sincos based on the number of raycasts
    {
        for( int i = 0; i < m_Raycasts; i++)
        {
            m_CosSin[i] = new Vector2(Mathf.Cos(i+1), Mathf.Sin(i+1)); //+1 because center is not calculated with cossin
        }
    }
    
    void ray_cast()  // clockwise raycasting, depending on the number of raycasts requested and forming the shape
    {
        float radii_segment = (Mathf.PI * 2) / m_Raycasts;
        m_Vlist.Clear();
        m_Vlist.Add(new RB_vert( new Vector2(0f,0f)));
        Vector2 gravity_ef = Physics2D.gravity * m_Gravity_Mult;
        Vector2 accel_ef = (rb.velocity - m_old_speed) * m_Accel_Mult;
        Vector2 position = new Vector2();

        int num_Vertices = 0;

        for (int i = 0; i < m_Raycasts; i++)
        {
            position.Set( radii_segment * m_Radius * m_CosSin[i].x , radii_segment * m_Radius * m_CosSin[i].y ); // position in circle
                                                                                                                 // determine how long is the raycast based on:
                                                                                                                 // acceleration
                                                                                                                 // gravity  Physics2D.gravity   * m_gravity_mult
                                                                                                                 // collider viscosity(physics property drag)   <<<---- Later on!
            m_Vlist.Add( new RB_vert( position ));
        }

        // Setup the Polygon Collider
    }

    Mesh Mesh_Maker()
    {
        Mesh new_mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = new_mesh;

        int mesh_verts = m_Drop_Vertices + 1;
        Vector3[] cozy_verts = new Vector3[mesh_verts];
        Vector2[] cozy_uv = new Vector2[mesh_verts];

        int mesh_triangle = mesh_verts * 3;
        int[] cozy_triang = new int[mesh_triangle];

        //        Array.Resize<Vector3>(ref cozy_verts, mesh_verts);
        //        Array.Resize<int>(ref cozy_triang, mesh_triangle);

        // building vertex [] and uv[]
        for (int i = 0; i < mesh_verts; i++)
        {
            cozy_verts[i] = (i == 0 ? new Vector3(0, 0, 0) : m_Vlist[i - 1].position);
            cozy_uv[i] = (i == 0 ? new Vector2(0, 0) : new Vector2(m_Vlist[i - 1].position.x, m_Vlist[i - 1].position.y));
        }

        //build triangle []
        for (int i = 0; i <= m_Drop_Vertices; i++)
        {
            cozy_triang[i * 3] = 0; // First vertex of every face is the center(0,0,0)
            cozy_triang[i * 3] = i; // Second vertes is the actual index
            cozy_triang[i * 3 + 1] = (i == m_Drop_Vertices) ? 1 : i + 1; // Third one is the next index, or the first one if it's the last index
        }
        new_mesh.vertices = cozy_verts;
        new_mesh.triangles = cozy_triang;
        new_mesh.uv = cozy_uv;

        return new_mesh;
    }
}
