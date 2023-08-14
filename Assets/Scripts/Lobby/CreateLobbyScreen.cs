using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateLobbyScreen : MonoBehaviour
{
    [SerializeField] private TMP_InputField _nameInput, _maxPlayersInput;
    [SerializeField] private TMP_Dropdown _roundDropdown;
    [SerializeField] private Button _createButton;
    [SerializeField] private Button _backButton;

    public static event Action<LobbyData> LobbyCreated;

    private void OnEnable()
    {
        _createButton.interactable = true;
        _maxPlayersInput.text = "5";
        _nameInput.text = NVJOBNameGen.Uppercase(NVJOBNameGen.GiveAName(7));

        SetOptions(_roundDropdown, Constants.Rounds);

        void SetOptions(TMP_Dropdown dropdown, IEnumerable<string> values)
        {
            dropdown.options = values.Select(type => new TMP_Dropdown.OptionData { text = type }).ToList();
        }

        _createButton.onClick.AddListener(() =>
        {
            OnCreateClicked();
            _createButton.interactable = false;
        });

        _maxPlayersInput.onValidateInput += delegate (string input, int charIndex, char addedChar) { return ValidateChar(addedChar); };
    }

    private void OnDisable()
    {
        _createButton.onClick.RemoveAllListeners();
        _maxPlayersInput.onValidateInput -= delegate (string input, int charIndex, char addedChar) { return ValidateChar(addedChar); };
    }

    private char ValidateChar(char charToValidate)
    {
        if (char.IsNumber(charToValidate)) return charToValidate;
        return '\0';
    }

    public void OnCreateClicked()
    {
        if (string.IsNullOrEmpty(_nameInput.text) || string.IsNullOrEmpty(_maxPlayersInput.text))
        {
            Debug.LogError("Name or max players is empty");
            return;
        }

        var lobbyData = new LobbyData
        {
            Name = _nameInput.text,
            MaxPlayers = int.Parse(_maxPlayersInput.text),
            Round = _roundDropdown.value
        };

        LobbyCreated?.Invoke(lobbyData);
    }

    public void OnBackButton()
    {
        _backButton.onClick.Invoke();
    }
}

public struct LobbyData
{
    public string Name;
    public int MaxPlayers;
    public int Round;
}