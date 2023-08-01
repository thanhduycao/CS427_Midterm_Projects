using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

public class MusicManager : MonoBehaviour
{
    public Sound[] s;
   

    public static Sound[] ss;

    public void Start()
    {
        ss = new Sound[s.Length];

        for (int i = 0; i < s.Length; i++)
        {
            ss[i] = s[i];

            ss[i].source = gameObject.AddComponent<AudioSource>();
            ss[i].source.clip = s[i].clip;
        }
        Debug.Log(ss);

    }

    public static void findMusic(string name)
    {
        
        Debug.Log(ss);
        Sound sou = Array.Find(ss, Sound => Sound.name == name);
        sou.source.Play();
    }

  
}
[System.Serializable]
public class Sound
{
    public string name;

    public AudioClip clip;
    public bool loop;
    public AudioSource source;

}