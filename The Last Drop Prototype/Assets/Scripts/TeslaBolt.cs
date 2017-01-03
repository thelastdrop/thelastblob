using UnityEngine;
using System.Collections;

public class TeslaBolt : MonoBehaviour
{
	
	private Renderer renderer;

	void Start ()
	{
		renderer = GetComponent<Renderer>();		
		Material newMat = renderer.material;
		newMat.SetFloat("_StartSeed",Random.value*1000);
		renderer.material = newMat;
		
	}

}

