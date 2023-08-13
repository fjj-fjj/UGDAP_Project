using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class audioManager : MonoBehaviour
{
    public static audioManager Instance;


    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource,sfxSource;

    private void Awake()
    {
        if(Instance==null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void PlayMusic(string name)
    {
        
        Sound s = Array.Find(musicSounds, x => x.name == name);
        if(s==null)
        {
            Debug.Log("Sound is not Found");
        }
        else
        {
            musicSource.clip = s.clip;
            musicSource.Play();
        }
    }

}
