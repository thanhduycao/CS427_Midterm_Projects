using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    private static SoundManager _instance;
    public SoundData[] sounds;
    private static Dictionary<SoundData.Sound, float> soundTimerDictionary;
    [SerializeField] private AudioSource backgroundSource;

    public static SoundManager instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        DontDestroyOnLoad(this.gameObject);

        soundTimerDictionary = new Dictionary<SoundData.Sound, float>();

        foreach (SoundData sound in sounds)
        {
            
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.audioClip;

            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.isLoop;

            if (sound.hasCooldown)
            {
                Debug.Log(sound.sound);
                soundTimerDictionary[sound.sound] = 0f;
            }
        }
    }

    private void Start()
    {
        // Add this part after having a theme song
        InitializeButtonSound();
        Play(SoundData.Sound.MainMenu);

    }
    public void Play(SoundData.Sound soundName)
    {
        SoundData sound = System.Array.Find(sounds, s => s.sound == soundName);

        if (sound == null)
        {
            Debug.LogError("Sound " + name + " Not Found!");
            return;
        }

        if (!CanPlaySound(sound)) return;

        Debug.Log("Here");
        sound.source.Play();
    }

    public void Stop(SoundData.Sound soundName)
    {
        SoundData sound = System.Array.Find(sounds, s => s.sound == soundName);

        if (sound == null)
        {
            Debug.LogError("Sound " + name + " Not Found!");
            return;
        }

        sound.source.Stop();
    }

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

    private void OnLevelWasLoaded(int level)
    {
        // Scene currentScene = SceneManager.GetActiveScene();
        // int sceneIndex = currentScene.buildIndex;
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

    private void InitializeButtonSound()
    {
        // Find all Button components in the scene
        Button[] buttons = FindObjectsOfType<Button>();

        // Attach the audio playback to each button's onClick event
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() => Play(SoundData.Sound.ButtonClick));
        }

    }
}
