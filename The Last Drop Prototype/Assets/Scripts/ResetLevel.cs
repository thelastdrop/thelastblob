using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ResetLevel : MonoBehaviour {



	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.Backspace)) {
			resetLevel ();

		}
	}

	void resetLevel() {
		SceneManager.LoadSceneAsync (SceneManager.GetActiveScene().name);
	}
		
}
