using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeslaCoil : MonoBehaviour {

	void OnCollisionEnter2D (Collision2D collision)
	{
		if (collision.gameObject.tag == "Player")
		{
			GameManager.Instance.m_Player.GetComponent<PlayerAvatar_02> ().Deactivate_Particle (collision.gameObject);
		}
	}
}
