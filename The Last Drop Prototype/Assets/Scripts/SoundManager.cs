using UnityEngine;
using System.Collections;

namespace Completed
{
    public class SoundManager : MonoBehaviour 
    {
		[Header("Effect sound")]
        public AudioSource efxSource;
		[Header("Music sound")]
        public AudioSource musicSource;
        public static SoundManager instance = null;
        public float lowPitchRange = .95f;              //The lowest a sound effect will be randomly pitched.
        public float highPitchRange = 1.05f;            //The highest a sound effect will be randomly pitched.
        
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

        public void PlayModPitch (AudioClip clip)
        {
            float randomPitch = Random.Range(lowPitchRange, highPitchRange);
            efxSource.pitch = randomPitch;
            efxSource.clip = clip;
            efxSource.Play();
        }
    }
}
