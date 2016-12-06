using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameWinManager : MonoBehaviour {
	
	public static GameWinManager Instance = null;
	private int current_level = 0;
	private int tutorial =0;


	[Header("EndLevel Screen")]
	public GameObject m_endlevel_screen;
	public Text m_ending_text;

	//[Header("Gameplay Screen")]
	//public GameObject m_timer_screen;
	//public Text m_timer_text;

	[Header("Choose-Levels Screen")]
	public GameObject m_levels_screen;


	[Header("Gameplay Screens")]
	public GameObject[] m_gameplay_screens;

	[Range(0f,4f)]
	private float m_current_time = 0;
	public float CurrentTime { get { return m_current_time; } } 


	[Header("Levels")]
	public Level[] m_levels;

	[Header("Levels accessible")]
	public bool[] m_levels_accessible;


	[Range(0f,4f)]
	public float m_loading_time = 0.5f;


	void Awake() {
		if (Instance == null) {
			Instance = this;
		} else {
			Destroy (gameObject);
		}
	}


	// Use this for initialization
	void Start () {
		//set all the levels except the first non accessible
		for (int i = 0; i < m_gameplay_screens.Length; i++) {
			if (i == tutorial) {
				m_levels_accessible [i] = true;
			} else {
				m_levels_accessible [i] = false;
			}
		}
			
		m_endlevel_screen.SetActive (false);
		m_levels_screen.SetActive (true);

		for (int i = 0; i < m_gameplay_screens.Length; i++) {
			m_gameplay_screens [i].SetActive (false);
		}
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}



	//bind this with the buttons of every level
	public void ChooseLevel(int n){
		if (m_levels_accessible [n]){
			current_level = n;
			StartCoroutine (LoadLevel ());
		}
		//else it does nothing and the button doesn't work
	
	}




	IEnumerator LoadLevel(){

		yield return new WaitForSeconds (m_loading_time);
		m_levels_screen.SetActive (false);
		//TODO MOVE current_level = n;

		m_gameplay_screens [current_level].SetActive (true);

		for (int i = 0; i < m_gameplay_screens.Length; i++) {
			if(current_level!=i)
				m_gameplay_screens [i].SetActive (false);
		}
	
	
	}


	//use with the button play next level
	public void NextLevel() {
		current_level++;
		m_levels_accessible [current_level] = true;
		LoadLevel ();

	}



	//use with the button play again
	public void ReloadLevel() {
			StartCoroutine (LoadLevel ());
			//TODO ADD EVENT MANAGER? EventManager.TriggerEvent ("EndLevel");
		}


	}