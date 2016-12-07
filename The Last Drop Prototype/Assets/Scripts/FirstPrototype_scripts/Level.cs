using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Level : ScriptableObject {

	[Range(0,100)]
	public float m_level_time = 0;

	public string m_name;

	void OnEnable() {
		/// do we need to init anyhow?
	}

	void OnDisable() {
		/// do we need to destroy/disable something?
	}



}
