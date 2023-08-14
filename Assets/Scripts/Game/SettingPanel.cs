using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : MonoBehaviour
{
    [SerializeField] private Button m_QuitButton;
    [SerializeField] private Slider m_MusicSlider, m_SFXSlider;
    [SerializeField] private Button m_ToggleMusic, m_ToggleSFX;

    public Action OnQuitButtonClicked;
    public Action OnOutSideClicked;

    private int xmin, xmax, ymin, ymax;

    private void Start()
    {
        m_QuitButton.onClick.AddListener(() =>
        {
            OnQuitButtonClicked?.Invoke();
        });

        // get the object size
        var rectTransform = GetComponent<RectTransform>();
        var sizeDelta = rectTransform.sizeDelta;
        var width = sizeDelta.x;
        var height = sizeDelta.y;

        var position = rectTransform.position;
        var x = position.x;
        var y = position.y;

        // calculate the min and max position
        xmin = (int)(x - width / 2);
        xmax = (int)(x + width / 2);
        ymin = (int)(y - height / 2);
        ymax = (int)(y + height / 2);

        m_MusicSlider.value = SoundManager.instance.GetMusicVolume();
        m_SFXSlider.value = SoundManager.instance.GetSFXVolume();

        m_MusicSlider.onValueChanged.AddListener((value) =>
        {
            SoundManager.instance.ChangeMusicVolume(value);
        });
        m_SFXSlider.onValueChanged.AddListener((value) =>
        {
            SoundManager.instance.ChangeSFXVolume(value);
        });

        m_ToggleMusic.onClick.AddListener(() =>
        {
            SoundManager.instance.ToggleMusic();
        });

        m_ToggleSFX.onClick.AddListener(() =>
        {
            SoundManager.instance.ToggleSFX();
        });
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var mousePosition = Input.mousePosition;
            if (mousePosition.x < xmin || mousePosition.x > xmax || mousePosition.y < ymin || mousePosition.y > ymax)
            {
                OnOutSideClicked?.Invoke();
            }
        }
    }
}
