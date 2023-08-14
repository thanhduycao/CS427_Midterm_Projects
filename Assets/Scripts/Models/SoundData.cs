using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SoundData
{
    public enum Sound
    {
        PlayerMove,
        PlayerJump,
        PlayerHit,
        Saw,
        ButtonClick,
        StartScene,
        EndScene,
        Dialog,
        Victory,
        MainMenu,
        Lobby,
        Scene1,
        Scene2,
        Scene3
    }

    public enum Round
    {
        Round1,
        Round2,
        Round3,
        None
    }
    // Start is called before the first frame update
    public Sound sound;
    public AudioClip audioClip;

    [Range(0f, 1f)]
    public float volume = 1f;

    [Range(.1f, 3f)]
    public float pitch = 1f;

    public bool isLoop;
    public bool hasCooldown;
    public Round roundSpecific = Round.None;
    public AudioSource source;
}
