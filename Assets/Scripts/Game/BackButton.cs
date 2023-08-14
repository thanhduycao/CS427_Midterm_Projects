using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackButton : MonoBehaviour
{

    public void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnBackButtonClicked);
    }

    public void OnBackButtonClicked()
    {
        FindFirstObjectByType<SettingButtonController>().OnQuitSetting();
    }
}
