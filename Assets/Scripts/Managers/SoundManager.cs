// Import required namespaces
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Define the SoundManager class
public class SoundManager : MonoBehaviour
{
    // Singleton instance
    private static SoundManager _instance;

    // Array of sound data
    public SoundData[] sounds;

    // Dictionary to store sound cooldown timers
    private static Dictionary<SoundData.Sound, float> soundTimerDictionary;

    // Reference to the background audio source
    [SerializeField] private AudioSource backgroundSource;

    // Lists to store music and sound effect audio sources
    private List<AudioSource> musicSource = new List<AudioSource>();
    private List<AudioSource> sfxSource = new List<AudioSource>();

    // Variables for music and sound effect volumes
    private float _musicVolume = 1;
    private float _sfxVolume = 1;

    // Singleton property
    public static SoundManager instance
    {
        get; private set;
    }

    // Awake method executed when the script is loaded
    private void Awake()
    {
        // Singleton check
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            // Initialize singleton instance and prevent it from being destroyed on scene changes
            instance = this;
            DontDestroyOnLoad(this.gameObject);

            // Initialize the soundTimerDictionary
            soundTimerDictionary = new Dictionary<SoundData.Sound, float>();

            // Loop through sound data and set up audio sources
            foreach (SoundData sound in sounds)
            {
                if (sound.source == null)
                {
                    sound.source = gameObject.AddComponent<AudioSource>();
                }
                sound.source.clip = sound.audioClip;

                sound.source.volume = sound.volume;
                sound.source.pitch = sound.pitch;
                sound.source.loop = sound.isLoop;

                // Set up sound cooldown if applicable
                if (sound.hasCooldown)
                {
                    soundTimerDictionary[sound.sound] = 0f;
                }

                // Categorize audio sources as music or sound effects
                if (sound.isMusicSound == true)
                {
                    musicSource.Add(sound.source);
                }
                else
                {
                    sfxSource.Add(sound.source);
                }
            }
        }
    }

    // Start method executed on the first frame
    private void Start()
    {
        // Initialize button sound and play the main menu sound
        InitializeButtonSound();
        Play(SoundData.Sound.MainMenu);
    }

    // Play a sound by name
    public void Play(SoundData.Sound soundName)
    {
        SoundData sound = System.Array.Find(sounds, s => s.sound == soundName);

        if (sound == null)
        {
            Debug.LogError("Sound " + name + " Not Found!");
            return;
        }

        if (!CanPlaySound(sound)) return;

        // Play the selected sound
        sound.source.Play();
    }

    // Stop a sound by name
    public void Stop(SoundData.Sound soundName)
    {
        SoundData sound = System.Array.Find(sounds, s => s.sound == soundName);

        if (sound == null)
        {
            Debug.LogError("Sound " + name + " Not Found!");
            return;
        }

        // Stop the selected sound
        sound.source.Stop();
    }

    // Check if a sound can be played based on cooldown
    private static bool CanPlaySound(SoundData sound)
    {
        if (soundTimerDictionary.ContainsKey(sound.sound))
        {
            float lastTimePlayed = soundTimerDictionary[sound.sound];

            if (lastTimePlayed + sound.audioClip.length < Time.time)
            {
                soundTimerDictionary[sound.sound] = Time.time;
                return true;
            }

            return false;
        }

        return true;
    }

    // Called when a new level is loaded
    private void OnLevelWasLoaded(int level)
    {
        // Initialize button sound and handle scene-specific audio
        InitializeButtonSound();
        switch (level)
        {
            case 3:
                Stop(SoundData.Sound.MainMenu);
                Play(SoundData.Sound.Scene1);
                break;
            case 4:
                Stop(SoundData.Sound.Scene1);
                Play(SoundData.Sound.Scene2);
                break;
            case 5:
                Stop(SoundData.Sound.Scene2);
                Play(SoundData.Sound.Scene3);
                break;
            default:
                Debug.Log("Error: No sound found");
                break;
        }
        Debug.Log(level);
    }

    // Attach button click sound to all buttons in the scene
    private void InitializeButtonSound()
    {
        Button[] buttons = FindObjectsOfType<Button>();

        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() => Play(SoundData.Sound.ButtonClick));
        }
    }

    // Toggle music playback
    public void ToggleMusic()
    {
        foreach (AudioSource source in musicSource)
        {
            source.mute = !source.mute;
        }
    }

    // Toggle sound effect playback
    public void ToggleSFX()
    {
        foreach (AudioSource source in sfxSource)
        {
            source.mute = !source.mute;
        }
    }

    // Change music volume
    public void ChangeMusicVolume(float volume)
    {
        _musicVolume = volume;
        foreach (AudioSource source in musicSource)
        {
            source.volume = volume;
        }
    }

    // Change sound effect volume
    public void ChangeSFXVolume(float volume)
    {
        _sfxVolume = volume;
        foreach (AudioSource source in sfxSource)
        {
            source.volume = volume;
        }
    }

    // Get current music volume
    public float GetMusicVolume()
    {
        return _musicVolume;
    }

    // Get current sound effect volume
    public float GetSFXVolume()
    {
        return _sfxVolume;
    }
}
