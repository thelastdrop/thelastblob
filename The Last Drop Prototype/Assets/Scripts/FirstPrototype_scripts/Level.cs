using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Level : ScriptableObject {

	[Range(0,100)]
	public float m_level_time = 0;



	public string m_name;


	public void Activate(){
		/*Debug.Log ("Activating Level " + m_name);
		m_screen.SetActive(false);
		Debug.Log ("Level " + m_name + ": " + m_screen.activeSelf);
		m_screen.SetActive(true);
		Debug.Log ("Level " + m_name + ": " + m_screen.activeSelf);*/

	}


	public void DeActivate(){
		/*Debug.Log ("DeActivating Level " + m_name);
		m_screen.gameObject.SetActive (false);
		Debug.Log ("Level " + m_name + ": " + m_screen.activeSelf);*/
	}

	void Start () {
		/*m_screen = GameObject.FindGameObjectWithTag ("GameplayScreen1") as GameObject;
		m_screen.SetActive (false);*/
	}


}
