using UnityEngine;
using System.Collections;

namespace Completed
{
    public class SoundManager : MonoBehaviour 
    {
        public AudioSource efxSource;
        public AudioSource musicSource;
        public static SoundManager instance = null;        
        
        void Awake ()
        {
            // Singleton pattern
            if (instance == null)
                instance = this;
            else if (instance != this)
                Destroy (gameObject);
            DontDestroyOnLoad (gameObject);
        }
        
        
        //Used to play single sound clips.
        public void PlaySingle(AudioClip clip)
        {
            efxSource.clip = clip;          
            efxSource.Play ();
        }
    }
}
