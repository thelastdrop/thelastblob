using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour {

    private bool m_used;

    void OnTriggerEnter2D( Collider2D other )
    {
        // ignore anything that is not the player, ignore anything after first activation
        // (to avoid using it multiple time on same frame, because of blob logic.. and reasons)
        if( (other.gameObject.tag != "Player") ||
            (m_used == true)                       )
        {

            return;
        }

        // Manager method that change position of start
        m_used = true;
        GameManager.Instance.CheckPoint(gameObject.transform.position);
        gameObject.SetActive(false);
    }
	
}
