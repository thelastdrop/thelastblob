using UnityEngine;
using System.Collections;
using POLIMIGameCollective;

public class SoundManager : Singleton<SoundManager>
{
    [Header("Volumes Prefs")]
    public float m_Master_Volume = 1.0f;
    public float m_Music_Volume = 1.0f;
    public float m_Sfx_Volume = 1.0f;
    public float m_UI_Volume = 1.0f;

    [Header("Music sound")]
    public AudioClip m_Music_Loop;

    [Header("Blob Sound")]
    public AudioClip m_Blob_Loop;

    public float lowPitchRange = 0.95f;
    public float highPitchRange = 1.05f;

    private AudioSource[] m_AudioS;

    [Header("Blob Sounds"),Space(5)]
    public AudioClip[] m_Blob_Damage;


    [Tooltip("Maximum absolute distance at which player ear the sound"),Header("Global Sound Variable"), Space(5)]
    public float m_Max_Distance = 50f;
    public AnimationCurve m_Sound_Falloff;

    private bool m_IsTime_Frozen = true;

    //Awake
    public void Start()
    {
        //    Table of AudioSource
        // 0  --- Music
        // 1  --- UI
        // *----* From now on are SFX
        // 2  --- Enemies
        // 3  --- Blob Damage
        // 4  --- Blob Movement
        // 5  --- Level Interactive items
        // 6  --- Screaming and deaths
        m_AudioS = GetComponents<AudioSource>();
        PlayMusic();
//        PlayBlob();
    }
/*
    void Update()
    {
        if(Time.timeScale == 0.0f && m_IsTime_Frozen == false)
        {
            PauseBlob();
            m_IsTime_Frozen = true;
        }

        if(Time.timeScale != 0.0f && m_IsTime_Frozen == true)
        {
            m_IsTime_Frozen = false;
            PlayAgainBlob();
        }
    }
*/

    //Play the music(channel 0)
    public void PlayMusic()
    {
        play_sound(0, m_Music_Loop, false, 0f);
    }

    //Used to play single sound clips.
    public void PlaySingle(AudioClip clip)
    {
        play_sound(1, clip, true, 0f);
    }

    public void PlayModPitch(AudioClip clip)
    {
        play_sound(2, clip, true, 0f);
    }

    public void PlayBlobDamage()
    {
        play_sound(3, m_Blob_Damage[Random.Range(0, m_Blob_Damage.Length - 1)], true, 0f);
    }

    public void PlayBlob()
    {
        play_sound( 4, m_Blob_Loop, false, 0f);
    }

    public void PauseBlob()
    {
        m_AudioS[4].Pause();
    }

    public void PlayAgainBlob()
    {
        m_AudioS[4].Play();
    }

    public void PlayLevelSound( AudioClip clip, bool rnd_pitch, float distance)
    {
        play_sound(5, clip, rnd_pitch, distance);
    }

    public void PlayDeathSound( AudioClip clip )
    {
        play_sound(6, clip, true, 0f);
    }

    private void play_sound( int source, AudioClip clip, bool rnd_pitch, float distance )
    {
        m_AudioS[source].clip = clip;

        if (source == 0)  // music
        {
            m_AudioS[source].loop = true;
            m_AudioS[source].volume = m_Master_Volume * m_Music_Volume;
        }

        if (source == 1) // UI
        {
            m_AudioS[source].loop = false;
            m_AudioS[source].volume = m_Master_Volume * m_UI_Volume;
        }

        if (source >= 2) // Other sounds... various Sfx
        {
            m_AudioS[source].loop = false;
            m_AudioS[source].volume = m_Master_Volume * m_Sfx_Volume;
        }

        if (source == 4 )
        {
            m_AudioS[source].loop = true;
            m_AudioS[source].volume = m_Master_Volume * m_Sfx_Volume * 0.4f;
        }

        if(rnd_pitch == true)
        {
            float randomPitch = Random.Range(lowPitchRange, highPitchRange);
            m_AudioS[source].pitch = randomPitch;
        } else
        {
            m_AudioS[source].pitch = 1.0f;
        }

        if(distance != 0f)
        {
            float curve_time = Mathf.Clamp((m_Max_Distance - distance), 0f, m_Max_Distance) / m_Max_Distance;
            m_AudioS[source].volume *= m_Sound_Falloff.Evaluate(curve_time);

//            Debug.Log("Distance: " + distance + "Volume:" + m_AudioS[source].volume);
        }
        
        m_AudioS[source].Play();
    }
}
