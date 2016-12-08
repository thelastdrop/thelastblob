using UnityEngine;
using System.Collections;

public class SoundManager : Singleton<SoundManager> {
		
        [Header("Effect sound")]
        public AudioSource efxSource;
		[Header("Music sound")]
        public AudioSource musicSource;
        public static SoundManager instance = null;
        public float lowPitchRange = .95f;              //The lowest a sound effect will be randomly pitched.
        public float highPitchRange = 1.05f;            //The highest a sound effect will be randomly pitched.
              
        
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
