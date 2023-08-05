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
    [SerializeField] private RawImage _colorPreview;

    private void Awake()
    {
        _colorPreview.color = CurrenPlayerData.Instance.GetColor();
        // clear all prefabs
        foreach (Transform child in _colorGridLayoutGroup.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var color in Constants.Colors)
        {
            var colorPrefab = Instantiate(_colorPrefab, _colorGridLayoutGroup.transform);
            var rawImage = colorPrefab.GetComponent<RawImage>();
            rawImage.color = color;
            colorPrefab.GetComponent<Button>().onClick.AddListener(() =>
            {
                _lobbyOrchestrator.SetColor(color: color);
                _colorPreview.color = color;
                CurrenPlayerData.Instance.SetColor(color);
            });
        }
        // set default
        _lobbyOrchestrator.SetName(NVJOBNameGen.Uppercase(NVJOBNameGen.GiveAName(7)));
        _lobbyOrchestrator.SetColor(color: Color.white);
    }

    private void Start()
    {
        _playerNameInputField.text = NVJOBNameGen.Uppercase(NVJOBNameGen.GiveAName(7));
    }

    public void OnPlayerNameEntered()
    {
        CurrenPlayerData.Instance.SetName(_playerNameInputField.text);
        _lobbyOrchestrator.SetName(name: _playerNameInputField.text);
    }
}
