using UnityEngine;
using UnityEngine.UI;

public class PlayerSetting : MonoBehaviour
{
    [Header("Lobby Orchestrator")]
    [SerializeField] private LobbyOrchestrator _lobbyOrchestrator;
    [Header("Player Settings")]
    [SerializeField] private GameObject _colorPrefab;
    [SerializeField] private TMPro.TMP_InputField _playerNameInputField;
    [SerializeField] private GridLayoutGroup _colorGridLayoutGroup;

    private void Awake()
    {
        // clear all prefabs
        foreach (Transform child in _colorGridLayoutGroup.transform)
        {
            Destroy(child.gameObject);
        }

        // add new prefabs
        // get color from Constants.Colors
        foreach (var color in Constants.Colors)
        {
            var colorPrefab = Instantiate(_colorPrefab, _colorGridLayoutGroup.transform);
            var rawImage = colorPrefab.GetComponent<RawImage>();
            rawImage.color = color;
            colorPrefab.GetComponent<Button>().onClick.AddListener(() =>
            {
                PlayerPrefs.SetString("PlayerColor", color.ToString());
                _lobbyOrchestrator.SetColor(color: color);
                CurrenPlayerData.Instance.SetColor(color);
            });
        }

        // set default
        _lobbyOrchestrator.SetName(PlayerPrefs.GetString("PlayerName", "Player"));
        string colorString = PlayerPrefs.GetString("PlayerColor", Color.white.ToString());
        try
        {
            ColorUtility.TryParseHtmlString(colorString, out Color _color);
            _lobbyOrchestrator.SetColor(color: _color);
        }
        catch (System.Exception) { }
    }

    private void Start()
    {
        _playerNameInputField.text = PlayerPrefs.GetString("PlayerName", "Player");
    }

    public void OnPlayerNameEntered()
    {
        PlayerPrefs.SetString("PlayerName", _playerNameInputField.text);
        CurrenPlayerData.Instance.SetName(_playerNameInputField.text);
        _lobbyOrchestrator.SetName(name: _playerNameInputField.text);
    }
}
