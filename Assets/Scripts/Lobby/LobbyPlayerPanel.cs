using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayerPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text _nameText, _statusText;
    [SerializeField] private RawImage _rawImage;

    public ulong PlayerId { get; private set; }


    public void Init(ulong playerId, PlayerData playerData)
    {
        PlayerId = playerId;
        Set(playerData);
    }

    public void Set(PlayerData playerData)
    {
        SetReady(playerData.Ready);
        SetName(playerData.Name);
        SetColor(playerData.Color);
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