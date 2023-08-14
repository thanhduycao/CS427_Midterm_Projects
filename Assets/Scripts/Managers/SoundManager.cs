// Import necessary Unity libraries.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// SoundManager class responsible for managing game sounds.
public class SoundManager : MonoBehaviour
{
    private static SoundManager _instance;
    
    // Array of SoundData objects that define different sounds in the game.
    public SoundData[] sounds;
    
    // Dictionary to keep track of sound cooldowns.
    private static Dictionary<SoundData.Sound, float> soundTimerDictionary;
    
    // Reference to the background music audio source.
    [SerializeField] private AudioSource backgroundSource;
    
    // Lists to store music and sound effect audio sources.
    private List<AudioSource> musicSource = new List<AudioSource>();
    private List<AudioSource> sfxSource = new List<AudioSource>();
    
    // Volume settings for music and sound effects.
    private float _musicVolume = 1;
    private float _sfxVolume = 1;

    // Singleton instance of SoundManager.
    public static SoundManager instance
    {
        get; private set;
    }

    // Awake is called before the Start method.
    private void Awake()
    {
        // Singleton pattern: Ensure only one instance of SoundManager exists.
        if (instance != null)
        {
            Destroy(this.gameObject); // Destroy duplicate instances.
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject); // Persist across scene changes.
            soundTimerDictionary = new Dictionary<SoundData.Sound, float>();
            InitializeSoundSources(); // Initialize audio sources from SoundData objects.
        }
    }

    // Start is called before the first frame update.
    private void Start()
    {
        InitializeButtonSound(); // Set up button click sounds.
        PlayMainMenuSound(); // Play main menu sound.
    }

    // Initialize audio sources based on SoundData objects.
    private void InitializeSoundSources()
    {
        foreach (SoundData sound in sounds)
        {
            InitializeSound(sound);
            CategorizeSoundSource(sound);
        }
    }

    // Initialize properties of an audio source based on a SoundData object.
    private void InitializeSound(SoundData sound)
    {
        if (sound.source == null)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
        }
        sound.source.clip = sound.audioClip;

        sound.source.volume = sound.volume;
        sound.source.pitch = sound.pitch;
        sound.source.loop = sound.isLoop;

        if (sound.hasCooldown)
        {
            soundTimerDictionary[sound.sound] = 0f;
        }
    }

    // Categorize an audio source as music or sound effect based on SoundData.
    private void CategorizeSoundSource(SoundData sound)
    {
        if (sound.isMusicSound)
        {
            musicSource.Add(sound.source);
        }
        else
        {
            sfxSource.Add(sound.source);
        }
    }

    // Play the main menu sound.
    private void PlayMainMenuSound()
    {
        InitializeButtonSound();
        Play(SoundData.Sound.MainMenu);
    }

    // Play a sound based on its name.
    public void Play(SoundData.Sound soundName)
    {
        SoundData sound = System.Array.Find(sounds, s => s.sound == soundName);
        if (sound == null)
        {
            Debug.LogError("Sound " + name + " Not Found!");
            return;
        }
        if (!CanPlaySound(sound)) return; // Check sound cooldown.
        sound.source.Play(); // Play the sound.
    }

    // Stop playing a sound based on its name.
    public void Stop(SoundData.Sound soundName)
    {
        SoundData sound = System.Array.Find(sounds, s => s.sound == soundName);
        if (sound == null)
        {
            Debug.LogError("Sound " + name + " Not Found!");
            return;
        }
        sound.source.Stop(); // Stop the sound.
    }

    // Check if a sound can be played based on its cooldown.
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

    // Called when a new level is loaded.
    private void OnLevelWasLoaded(int level)
    {
        Stop(SoundData.Sound.Scene1);
        Stop(SoundData.Sound.Scene2);
        Stop(SoundData.Sound.Scene3);
        InitializeButtonSound();
        // Play appropriate sounds based on the loaded level.
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

    // Set up button click sounds for all buttons.
    private void InitializeButtonSound()
    {
        Button[] buttons = FindObjectsOfType<Button>();
        foreach (Button button in buttons)
        {
            // Add a click sound to each button.
            button.onClick.AddListener(() => Play(SoundData.Sound.ButtonClick));
        }
    }

    // Toggle music on/off.
    public void ToggleMusic()
    {
        ToggleAudioSources(musicSource);
    }

    // Toggle sound effects on/off.
    public void ToggleSFX()
    {
        ToggleAudioSources(sfxSource);
    }

    // Toggle audio sources on/off.
    private void ToggleAudioSources(List<AudioSource> sources)
    {
        foreach (AudioSource source in sources)
        {
            source.mute = !source.mute;
        }
    }

    // Change the volume of music.
    public void ChangeMusicVolume(float volume)
    {
        ChangeVolume(musicSource, volume, ref _musicVolume);
    }

    // Change the volume of sound effects.
    public void ChangeSFXVolume(float volume)
    {
        ChangeVolume(sfxSource, volume, ref _sfxVolume);
    }

    // Change the volume of audio sources.
    private void ChangeVolume(List<AudioSource> sources, float volume, ref float targetVolume)
    {
        targetVolume = volume;
        foreach (AudioSource source in sources)
        {
            source.volume = volume;
        }
    }

    // Get the current music volume setting.
    public float GetMusicVolume()
    {
        return _musicVolume;
    }

    // Get the current sound effects volume setting.
    public float GetSFXVolume()
    {
        return _sfxVolume;
    }
}
