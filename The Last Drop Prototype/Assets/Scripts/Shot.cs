using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot : MonoBehaviour {

	[Range(20f, 150f)]
	public float m_speed = 50f;
	Transform tr;
	public float raycastMagnitude = 0.1f;
    private int layerMaskPlatforms = 1 << 8;

	void Start() {
		tr = GetComponent<Transform> () as Transform;
	}
	
	void FixedUpdate() {
		tr.position = tr.position + tr.right * m_speed * Time.fixedDeltaTime;
		RaycastHit2D hits = Physics2D.Raycast(tr.position, tr.right, raycastMagnitude, layerMaskPlatforms);
		if(hits != null)
		{
			gameObject.SetActive(false);
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
		gameObject.SetActive(false);
		if(other.gameObject.tag == "Player")
		{
			GameManager.Instance.m_Player.GetComponent<PlayerAvatar_02>().Deactivate_Particle(other.gameObject);
		}
	}
}
