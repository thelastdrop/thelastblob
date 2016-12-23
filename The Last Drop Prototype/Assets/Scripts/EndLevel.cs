﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLevel : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D collision)
    {
        if( collision.gameObject.tag == "Player" )
        {
            GameWinManager.Instance.WinLevel();
        }
    }
}
