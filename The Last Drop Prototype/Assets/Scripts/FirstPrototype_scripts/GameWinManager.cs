using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameWinManager : MonoBehaviour
{
	
	public static GameWinManager Instance = null;
	private int current_level = 0;
	private int tutorial = 0;


	[Header ("EndLevel Screen")]
	public GameObject m_endlevel_screen;
	public Text m_ending_text;


	[Header ("LoseLevel Screen")]
	public GameObject m_loselevel_screen;

	[Header ("PauseLevel Screen")]
	public GameObject m_pauselevel_screen;

	//[Header("Gameplay Screen")]
	//public GameObject m_timer_screen;
	//public Text m_timer_text;

	[Header ("Choose-Levels Screen")]
	public GameObject m_levels_screen;


	[Header ("Gameplay Screens")]
	public GameObject[] m_gameplay_screens;

	[Range (0f, 4f)]
	private float m_current_time = 0;

	public float CurrentTime { get { return m_current_time; } }


	[Header ("Levels")]
	public Level[] m_levels;

	[Header ("Levels accessible")]
	public bool[] m_levels_accessible;


	[Range (0f, 4f)]
	public float m_loading_time = 0.5f;

	[Header ("Gravity Input")]
	public Gravity gravityInput;


	private GameObject m_playing_screen;

	void Awake ()
	{
		if (Instance == null) {
			Instance = this;
		} else {
			Destroy (gameObject);
		}
	}


	// Use this for initialization
	void Start ()
	{
		//set all the levels except the first non accessible
		for (int i = 0; i < m_gameplay_screens.Length; i++) {
			if (i == tutorial) {
				m_levels_accessible [i] = true;
			} else {
				m_levels_accessible [i] = false;
			}
			m_gameplay_screens [i].SetActive (false);
		}
		ClearScreens ();

		m_levels_screen.SetActive (true);

	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Backspace)) {
			ReloadLevel ();
		}

		if (Input.GetKeyDown (KeyCode.P)) {
			PauseLevel ();
		}
	}



	//bind this with the buttons of every level
	public void ChooseLevel (int n)
	{
		if (m_levels_accessible [n]) {
			current_level = n;
			StartCoroutine (LoadLevel ());
		}
		//else it does nothing and the button doesn't work
	
	}




	IEnumerator LoadLevel ()
	{

		yield return new WaitForSeconds (m_loading_time);

		//initialization
		this.ClearScreens ();
		gravityInput.ResetGravity ();

		m_playing_screen = Instantiate (m_gameplay_screens [current_level]);
		m_playing_screen.SetActive (true);
	
	
	}


	//use with the button play next level
	public void NextLevel ()
	{
		current_level++;
		StartCoroutine (LoadLevel ());

	}



	//use with the button play again
	public void ReloadLevel ()
	{
		StartCoroutine (LoadLevel ());
	}


	public void WinLevel ()
	{
		if (current_level + 1 < m_levels_accessible.Length) {
			m_levels_accessible [current_level + 1] = true;
		}
		this.EndLevel ();
		m_endlevel_screen.SetActive (true);

	}
		

	void EndLevel()
	{
		this.ClearScreens ();
		Destroy (m_playing_screen);
		
	}


	public void LoseLevel()
	{
		this.EndLevel ();
		m_loselevel_screen.SetActive (true);
	}


	public void PauseLevel()
	{
		m_playing_screen.SetActive (false);
		m_pauselevel_screen.SetActive (true);
	}


	public void ResumeLevel(){
		m_pauselevel_screen.SetActive (false);
		m_playing_screen.SetActive (true);
	}
		

	void ClearScreens ()
	{
		if (m_endlevel_screen != null)
			m_endlevel_screen.SetActive (false);
		if (m_levels_screen != null)
			m_levels_screen.SetActive (false);
		if (m_playing_screen != null)
			m_playing_screen.SetActive (false);
		if (m_loselevel_screen != null)
			m_loselevel_screen.SetActive (false);
		if (m_pauselevel_screen != null)
			m_pauselevel_screen.SetActive (false);
		
	}



	public void SwitchToMenu()
	{
		SceneManager.LoadScene ("Menu");
	}
		 

}