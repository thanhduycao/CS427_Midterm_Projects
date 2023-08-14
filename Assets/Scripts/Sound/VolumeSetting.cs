using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSetting : MonoBehaviour
{
    public Slider musicSlider, sfxSlider;

    public void ToggleMusic()
    {
        SoundManager.instance.ToggleMusic();
    }

    public void ToggleSFX()
    {
        SoundManager.instance.ToggleSFX();
    }

    public void ChangeMusicVolume()
    {
        SoundManager.instance.ChangeMusicVolume(musicSlider.value);
    }

    public void ChangeSFXVolume()
    {
        SoundManager.instance.ChangeSFXVolume(sfxSlider.value);
    }
}
