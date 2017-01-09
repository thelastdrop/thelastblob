﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using POLIMIGameCollective;

// This class manages all the game architecture, loading a level, pausing,  restarting, winning, losing
// Also keep in memory progress made by the player.

public class GameWinManager : Singleton<GameWinManager>
{

	// index of current level
	private int current_level = 0;


	// index of the first level accesible by the player at the first opening
	private int m_FirstUnlocked;


	[Header ("EndLevel Screen")]
	public GameObject m_endlevel_screen;
	[Header ("Loading Screen")]
	public GameObject m_loading_screen;
	[Header ("LoseLevel Screen")]
	public GameObject m_loselevel_screen;
	[Header ("PauseLevel Screen")]
	public GameObject m_pauselevel_screen;
	[Header ("Choose-Levels Screen")]
	public GameObject m_levels_screen;


	[Header ("PlayerAvatar")]
	public GameObject playerAvatar;



	/* array of gameplay screens, each index is one level, this objects are never used directly nor mutated,
	 * but cloned (Object.Instantiate) in m_playing_screen each time a new level is started to ensure a clean state */
	[Header ("Gameplay Screens")]
	public GameObject[] m_gameplay_screens;

	private GameObject m_playing_screen;

	//if timer is implemented use this
	[Range (0f, 4f)]
	private float m_current_time = 0;

	public float CurrentTime { get { return m_current_time; } }

	/* array of booleans with the same length of levels and same indexes, if a level is playable because the gamers 
	 * won it the m_levels_accessible[i] is true otherwise is false and that level isn't accessible */

	[Header ("Levels accessible")]
	public bool[] m_levels_accessible;

	[Header ("Buttons in Choose-Levels Screen")]
	public GameObject[] m_levels_buttons;


	[Header ("Loading time for Level")]
	[Range (0f, 4f)]
	public float m_loading_time = 0.5f;





	void Start ()
    {
        Time.timeScale = 0f;
        Activate_GamePlay_Level();
    }
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Backspace)) {
			ReloadLevel ();
		}

        if (Input.GetKeyDown (KeyCode.P) || Input.GetKeyDown (KeyCode.Menu) ) {
            ReloadLevel();
		}
	}

    //Modded by Renè, it requires to use this method when it's loading "Again" the scene. Start only fire once, if the
    //scene is then exited and re-entered start is not going to fire.
    public void Activate_GamePlay_Level()
    {
        // This will get the progress of the player, and setup the variable used to store his progress through the game
        Get_Player_Progress();

        foreach(GameObject elem in m_levels_buttons)
        {
            elem.SetActive(false);
        }

        //set all the levels up to m_First_Unlocked to active
        for (int i = 0; i < m_gameplay_screens.Length; i++)
        {
            /*  
              if (i <= m_FirstUnlocked)
              {
                  m_levels_accessible[i] = true;
                  m_levels_buttons[i].SetActive(true);
              }
              else
              {
                  //m_levels_accessible[i] = false;
              }
              */

            m_levels_accessible[i] = true;
            m_levels_buttons[i].SetActive(true);
            //deactivate all the level screens, they will never be used directly 
            m_gameplay_screens[i].SetActive(false);
        }

        ClearScreens();

        // activate level chooser
        m_levels_screen.SetActive(true);
        

    }

    //called when the user choses one level
    public void ChooseLevel (int n)
	{
            
		if (m_levels_accessible [n]) {
			current_level = n;
            Time.timeScale = 1.0f;
            StartCoroutine (LoadLevel ());
            Debug.Log(n);
        }
		//else it does nothing and the button doesn't work
	
	}



	// loads the current level

	IEnumerator LoadLevel ()
	{


        yield return new WaitForSeconds (m_loading_time);

        //initialization
        this.ClearScreens ();
        //playerAvatar.SetActive (true);


        if (m_playing_screen != null) Destroy( m_playing_screen );

        //duplicate the required level and activate it
        m_playing_screen = Instantiate ( m_gameplay_screens [current_level] );
		m_playing_screen.SetActive (true);
        playerAvatar.GetComponent<PlayerAvatar_02>().PlayerReset( GameManager.Instance.m_Player_Restart_Particles);
        GameManager.Instance.Gravity_Change( GameManager.Instance.m_Restart_Gravity_Ind );
        SoundManager.Instance.PlayMusic();

        POLIMIGameCollective.EventManager.TriggerEvent("LoadLevel");
    }


	//triggered by the button "next level"
	public void NextLevel ()
	{
		current_level++;
        Time.timeScale = 1.0f;
        StartCoroutine (LoadLevel ());

	}



	//triggered by the button "play again" in Lose/Win screens
	public void ReloadLevel ()
    {
        Time.timeScale = 1.0f;
        StartCoroutine (LoadLevel ());
	}





	//called when the player reaches the end of the level
	public void WinLevel ()
    {
        Time.timeScale = 0f;
 //       this.EndLevel ();


        // set as accessible (true) the next level if the current one is won
        if (current_level + 1 < m_levels_accessible.Length) {
			m_levels_accessible [current_level + 1] = true;
			m_levels_buttons [current_level + 1].SetActive (true);
            save_progress();
		}
		m_endlevel_screen.SetActive (true);

	}


	// called to destroy the current level screen
	// never called directly by the UI
	void EndLevel ()
	{
		POLIMIGameCollective.EventManager.TriggerEvent ("EndLevel");
		playerAvatar.GetComponent<PlayerAvatar_02> ().PlayerReset ( 5 );
		//TODO check if it is correct
		//Time.timeScale = 0f;
		//playerAvatar.SetActive (false);
		this.ClearScreens ();
        // destroy the currently allocated level screen when a level ends winning/losing
        Destroy(m_playing_screen);

    }


	//called when the player loses in a level
	public void LoseLevel ()
    {
        Time.timeScale = 0f;
        if (m_loselevel_screen != null)
        {
            m_loselevel_screen.SetActive(true);
        } else
        {
            Debug.Log("No LoseLevel Screen is set! Add reference to a UI for losing level!");
        }
    }


	//called when the player pauses the game
	public void PauseLevel ()
	{
		//POLIMIGameCollective.EventManager.TriggerEvent ("PauseLevel");
		//playerAvatar.SetActive (false);

		//TODO check if it is correct
		Time.timeScale = 0f;
//		m_playing_screen.SetActive (false);
		m_pauselevel_screen.SetActive (true);
	}


	//triggered by the button "continue" in the pause screen
	public void ResumeLevel ()
	{
		//POLIMIGameCollective.EventManager.TriggerEvent ("ResumeLevel");
		//playerAvatar.SetActive (true);
		//TODO check if it is correct
		Time.timeScale = 1f;
		m_pauselevel_screen.SetActive (false);
		m_playing_screen.SetActive (true);
	}


	public void ListLevels ()
	{
		this.EndLevel ();
		m_levels_screen.SetActive (true);
	}



	public void LoadingLevel ()
	{
		this.EndLevel ();
		m_loading_screen.SetActive (true);
		this.ReloadLevel ();
	}


	// deactivate all the screens
	void ClearScreens ()
	{
		if (m_endlevel_screen != null)
			m_endlevel_screen.SetActive (false);
		if (m_levels_screen != null)
			m_levels_screen.SetActive (false);
//		if (m_playing_screen != null)
//			m_playing_screen.SetActive (false);
		if (m_loselevel_screen != null)
			m_loselevel_screen.SetActive (false);
		if (m_pauselevel_screen != null)
			m_pauselevel_screen.SetActive (false);
		if (m_loading_screen != null)
			m_loading_screen.SetActive (false);
		
	}



	//come back to main menu
	public void SwitchToMenu ()
	{
        Time.timeScale = 1.0f;
		SceneManager.LoadScene ("Menu");
	}

    void Get_Player_Progress()
    {
        if (PlayerPrefs.HasKey("LevelUnlocked"))
        {
            m_FirstUnlocked = PlayerPrefs.GetInt("LevelUnlocked");
        } else
        {
            PlayerPrefs.SetInt("LevelUnlocked", 0);
            m_FirstUnlocked = 0;
            PlayerPrefs.Save();
        }
    }


    void save_progress()
    {
        PlayerPrefs.SetInt("LevelUnlocked", current_level + 1);
        PlayerPrefs.Save();
    }

}