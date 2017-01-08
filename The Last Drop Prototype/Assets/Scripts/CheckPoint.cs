using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour {

    [Tooltip("Number of particle the player will have on spawning form this checkpoint")]
    public int m_Num_Particles = 7;

    [Tooltip("Gravity index, general direction of gravity when starting from this checkpoint(0-3)")]
    public int m_Gravity_Index = 0;

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
        GameManager.Instance.CheckPoint( gameObject.transform.position, m_Num_Particles, m_Gravity_Index);
        gameObject.SetActive(false);
    }
	
}
