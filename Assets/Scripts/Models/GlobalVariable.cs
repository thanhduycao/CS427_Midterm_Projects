using UnityEngine;

public class GlobalVariable : MonoBehaviour
{
    public static GlobalVariable Instance { get; private set; }

    [SerializeField] private ConfigAvatarData m_AvatarData;
    [SerializeField] private PlayerController m_PlayerPrefab;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            if (m_AvatarData == null)
            {
                string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                Debug.LogError("Avatar data is null");
                Debug.LogError($"Please add avatar data to GlobalVariable at scene {sceneName} with path Assets/Configs/AvatarData.asset");
            }

            if (m_PlayerPrefab == null)
            {
                string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                Debug.LogError("Player prefab is null");
                Debug.LogError($"Please add player prefab to GlobalVariable at scene {sceneName} with path Assets/Prefabs/Player.prefab");
            }

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private int gameMode = 0; // 0: offline, 1: online
    private int round = 0; // round number
    private bool onReload = false;

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

    public bool OnReload
    {
        get => onReload; set { onReload = value; if (onReload) CurrenPlayerData.Instance.Ready = false; }
    }

    public PlayerController PlayerPrefab => m_PlayerPrefab;

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
