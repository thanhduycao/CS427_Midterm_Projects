using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayerPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text _nameText, _statusText;
    [SerializeField] private RawImage _rawImage;

    public ulong PlayerId { get; private set; }


    public void Init(ulong playerId, LobbyPlayerData playerData)
    {
        PlayerId = playerId;
        SetReady(playerData.ready);
        SetName(playerData.name);
        SetColor(playerData.color);
    }

    public void Set(LobbyPlayerData playerData)
    {
        SetReady(playerData.ready);
        SetName(playerData.name);
        SetColor(playerData.color);
    }

    public void SetReady(bool ready)
    {
        if (ready)
        {
            _statusText.text = "Ready";
            _statusText.color = Color.green;
        }
        else
        {
            _statusText.text = "Ready";
            _statusText.color = Color.red;
        }
    }

    public void SetName(string name)
    {
        _nameText.text = name;
    }

    public void SetColor(Color color)
    {
        _rawImage.color = color;
    }
}