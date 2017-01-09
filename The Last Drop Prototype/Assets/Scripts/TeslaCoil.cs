using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeslaCoil : MonoBehaviour {

	public AudioClip[] audioclips;

	void OnTriggerEnter2D (Collider2D collider)
	{
		if (collider.gameObject.tag == "Player")
		{
			GameManager.Instance.m_Player.GetComponent<PlayerAvatar_02> ().Deactivate_Particle(collider.gameObject);
			int randomClipIndex = (int)Mathf.Floor(Random.Range(0, audioclips.Length - 1));
            SoundManager.Instance.PlayModPitch(audioclips[randomClipIndex]);
		}
	}
}
