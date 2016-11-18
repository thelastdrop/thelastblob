using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
    public static GameManager Instance = null;

    [Header("Prefab for object pooling")]
    public GameObject m_dynam_particle;

    void Awake()
    {
        if( Instance == null)
        {
            Instance = this;
        } else
        {
            Destroy(gameObject);
        }
    }

	// Use this for initialization
	void Start () {
        ObjectPoolingManager.Instance.CreatePool(m_dynam_particle, 130, 130);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
