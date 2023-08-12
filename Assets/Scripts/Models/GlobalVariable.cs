using UnityEngine;

public class GlobalVariable : MonoBehaviour
{
    public static GlobalVariable Instance { get; private set; }

    [SerializeField] private ConfigAvatarData m_AvatarData;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private int gameMode = 0; // 0: offline, 1: online
    private int round = 0; // round number

    public int GameMode { get => gameMode; set => gameMode = value; }
    public int Round
    {
        get => round;
        set
        {
            if (value >= 0 && value < Constants.Rounds.Count) round = value;
            else round = -1;
        }
    }

    public AvatarData[] Avatars => m_AvatarData.avatars;

    public AvatarData GetAvatar(int avatar)
    {
        return m_AvatarData.GetAvatar(avatar);
    }

    public int AvatarCount()
    {
        return m_AvatarData.Count();
    }
}
