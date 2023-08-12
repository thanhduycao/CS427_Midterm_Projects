using UnityEngine;
using UnityEngine.UI;

public class HealthPlayerPanel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ulong m_PlayerId;
    [SerializeField] private TMPro.TMP_Text m_PlayerName;
    [SerializeField] private HealthBar m_HealthBar;
    [SerializeField] private RawImage m_RawImage;

    [Header("Settings")]
    [SerializeField] private int m_MaxHealth = 100;
    [SerializeField] private int m_Health = 100;

    private void Start()
    {
        m_HealthBar.SetMaxHealth(m_MaxHealth);
    }

    private void FixedUpdate()
    {
        m_HealthBar.SetHealth(m_Health);
    }

    public void Set(PlayerState playerState)
    {
        //SetPlayerId(playerState.Id);
        SetPlayerName(playerState.Name);
        SetPlayerColor(playerState.Color);
        SetPlayerHealth(playerState.Health);
        SetPlayerAvatar(playerState.Avatar);
    }

    public void SetPlayerState(ulong playerId, PlayerState playerState)
    {
        if (m_PlayerId == playerId)
        {
            Set(playerState);
        }
    }

    public void SetPlayerName(string playerName)
    {
        m_PlayerName.text = playerName;
    }

    public void SetPlayerHealth(int health)
    {
        m_Health = health;
    }

    public void SetPlayerHealth(ulong playerId, int health)
    {
        if (m_PlayerId == playerId)
        {
            m_Health = health;
        }
    }

    public void SetPlayerImage(Sprite sprite)
    {
        m_RawImage.texture = sprite.texture;
    }

    public void SetPlayerColor(Color color)
    {
        m_PlayerName.color = color;
    }

    public ulong GetPlayerId()
    {
        return m_PlayerId;
    }

    public void SetPlayerId(ulong playerId)
    {
        m_PlayerId = playerId;
    }

    public void SetPlayerAvatar(int avatar)
    {
        AvatarData _avatar = GlobalVariable.Instance.GetAvatar(avatar);
        SetPlayerImage(_avatar.AvatarSprite);
    }
}
