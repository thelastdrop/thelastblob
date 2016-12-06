using UnityEngine;
using System.Collections;

public class Dynam_Particle : MonoBehaviour
{

    public Rigidbody2D rb;
    public enum STATES { WATER, GAS, LAVA, NONE }; //The 3 states of the particle
    STATES currentState = STATES.NONE; //Defines the currentstate of the particle, default is water
    public GameObject currentImage; //The image is for the metaball shader for the effect, it is onle seen by the liquids camera.
    public GameObject[] particleImages; //We need multiple particle images to reduce drawcalls
    float GAS_FLOATABILITY = 1.0f; //How fast does the gas goes up?
    float particleLifeTime = 3.0f, startTime;//How much time before the particle scalesdown and dies
    public bool scales_down = false;

    void start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        if (currentState == STATES.NONE)
            SetState(STATES.WATER);
    }

    void OnEnable()
    {
        startTime = Time.time;
    }

    void OnDisable()  // Reset state of the particle so that can be placed back to the pool
    {
        /*
        SpringJoint2D[] joints = gameObject.GetComponents<SpringJoint2D>();
        foreach( SpringJoint2D elem in joints )
        {
            Destroy(elem);
        }
        */
        UnityEditor.PrefabUtility.ResetToPrefabState(gameObject);
    }

    //The definitios to each state
    public void SetState(STATES newState)
    {
        if (newState != currentState)
        { //Only change to a different state
            switch (newState)
            {
                case STATES.WATER:
                    rb.gravityScale = 1.0f; // To simulate Water density
                    break;
                case STATES.GAS:
                    particleLifeTime = particleLifeTime / 2.0f; // Gas lives the time the other particles
                    rb.gravityScale = 0.0f;// To simulate Gas density
                    gameObject.layer = LayerMask.NameToLayer("Gas");// To have a different collision layer than the other particles (so gas doesnt rises up the lava but still collides with the wolrd)
                    break;
                case STATES.LAVA:
                    rb.gravityScale = 0.3f; // To simulate the lava density
                    break;
                case STATES.NONE:
                    gameObject.SetActive(false);
                    break;
            }
            if (newState != STATES.NONE)
            {
                currentState = newState;
                startTime = Time.time;//Reset the life of the particle on a state change
                rb.velocity = new Vector2();    // Reset the particle velocity	
                currentImage.SetActive(false);
                currentImage = particleImages[(int)currentState];
                currentImage.SetActive(true);
            }
        }
    }
    void Update()
    {
        switch (currentState)
        {
            case STATES.WATER: //Water and lava got the same behaviour
                MovementAnimation();
                if(scales_down) ScaleDown();
                break;
            case STATES.LAVA:
                MovementAnimation();
                if (scales_down) ScaleDown();
                break;
            case STATES.GAS:
                if (rb.velocity.y < 50)
                { //Limits the speed in Y to avoid reaching mach 7 in speed
                    rb.AddForce( -Physics2D.gravity * GAS_FLOATABILITY ); // Gas always goes upwards
                }
                if (scales_down) ScaleDown();
                break;

        }
    }
    // This scales the particle image acording to its velocity, so it looks like its deformable... but its not ;)
    void MovementAnimation()
    {
        Vector3 movementScale = new Vector3(1.0f, 1.0f, 1.0f);//Tamaño de textura no de metaball			
        movementScale.x += Mathf.Abs(rb.velocity.x) / 30.0f;
        movementScale.z += Mathf.Abs(rb.velocity.y) / 30.0f;
        movementScale.y = 1.0f;
        currentImage.gameObject.transform.localScale = movementScale;
    }
    // The effect for the particle to seem to fade away
    void ScaleDown()
    {
        float scaleValue = 1.0f - ((Time.time - startTime) / particleLifeTime);
        Vector2 particleScale = Vector2.one;
        if (scaleValue <= 0)
        {
            gameObject.SetActive(false);
        }
        else
        {
            particleScale.x = scaleValue;
            particleScale.y = scaleValue;
            transform.localScale = particleScale;
        }
    }

    // To change particles lifetime externally (like the particle generator)
    public void SetLifeTime(float time)
    {
        particleLifeTime = time;
    }
    // Here we handle the collision events with another particles, in this example water+lava= water-> gas
    void OnCollisionEnter2D(Collision2D other)
    {
        if (currentState == STATES.WATER && other.gameObject.tag == "DynamParticle")
        {
            if (other.collider.GetComponent<Dynam_Particle>().currentState == STATES.LAVA)
            {
                SetState(STATES.GAS);
            }
        }

    }

    public Rigidbody2D get_rb()
    {
        return rb;
    }

}