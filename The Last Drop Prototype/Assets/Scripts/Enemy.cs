﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	public float m_speed = 2f;

	private int verse = 1;
	private Transform tr;
	private Vector2 raycastDirection = Vector2.down; //new Vector2(1f,-1f);
	private SpriteRenderer sr;

	private Animator animator;
	private bool moving = false;

    void Start () {
        tr = GetComponent<Transform>() as Transform;
		sr = GetComponent<SpriteRenderer>() as SpriteRenderer;
		animator = GetComponent<Animator>() as Animator;
		animator.SetBool("Moving", moving);
    }

    void FixedUpdate () {
		Move();
    }

	void Move() {
		moving = true;
		RaycastHit2D[] hits = Physics2D.RaycastAll(tr.position, raycastDirection, 0.5f);

		// If there's no platform under this collider2D
		if(hits.Length <= 1) {
			verse *= -1;	// Move in other direction
			sr.flipX = verse > 0 ? false : true;	// Flip the sprite
			// raycastDirection = new Vector2(verse * 1f, -1f); // Flip the raycast direction
		}

		// Move
		tr.position = tr.position + verse * m_speed * transform.right * Time.fixedDeltaTime;
	}
}
