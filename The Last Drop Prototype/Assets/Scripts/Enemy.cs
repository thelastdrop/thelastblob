using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	public float m_speed = 2f;
	private int verse = 1;
	private Transform tr;
	private Collider2D col2D;
	private SpriteRenderer sr;

    void Start () {
        tr = GetComponent<Transform>() as Transform;
		col2D = GetComponent<Collider2D>() as Collider2D;
		sr = GetComponent<SpriteRenderer>() as SpriteRenderer;
    }

    void FixedUpdate () {
		// TODO
		RaycastHit2D[] hits = null;
		int hitsCount = col2D.Raycast(Vector2.down, hits);
		if(hitsCount > 0) {
			
		} else {
			verse *= -1;
			sr.flipX = verse > 0 ? false : true; 
		}
		tr.position = tr.position + verse * m_speed * transform.right * Time.fixedDeltaTime;
    }

	/*void OnCollisionExit2D(Collision2D coll) {
        // if (coll.gameObject.tag == "Platform")
        verse *= -1;
		sr.flipX = verse > 0 ? false : true; 
    }*/
}
