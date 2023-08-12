using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingButtonController : MonoBehaviour
{
    [SerializeField] private Transform m_Canvas;
    [SerializeField] private SettingPanel m_SettingPanelPrefab;

    private Button m_SettingButton;
    private SettingPanel settingPanel;

    void Start()
    {
        foreach (Transform child in transform) Destroy(child.gameObject);

        m_SettingButton = GetComponent<Button>();
        m_SettingButton.onClick.AddListener(() =>
        {
            if (settingPanel != null)
            {
                settingPanel.OnOutSideClicked -= OnCloseSetting;
                settingPanel.OnQuitButtonClicked -= OnQuitSetting;
                Destroy(settingPanel.gameObject);
                settingPanel = null;
            }
            else
            {
                settingPanel = Instantiate(m_SettingPanelPrefab, Vector3.zero, Quaternion.identity, m_Canvas);
                settingPanel.transform.localPosition = Vector3.zero;
                settingPanel.OnOutSideClicked += OnCloseSetting;
                settingPanel.OnQuitButtonClicked += OnQuitSetting;
            }
        });
    }

    private void OnDestroy()
    {
        m_SettingButton.onClick.RemoveAllListeners();
    }

    private void OnQuitSetting()
    {
        FindObjectOfType<GameManager>().OnDestroy();
        SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
    }

    private void OnCloseSetting()
    {
        if (settingPanel != null)
        {
            settingPanel.OnOutSideClicked -= OnCloseSetting;
            settingPanel.OnQuitButtonClicked -= OnQuitSetting;
            Destroy(settingPanel.gameObject);
            settingPanel = null;
        }
    }
}
