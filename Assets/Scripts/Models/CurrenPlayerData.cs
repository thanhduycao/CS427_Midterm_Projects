using Unity.Netcode;
using UnityEngine;

public class CurrenPlayerData : NetworkBehaviour
{
    public static CurrenPlayerData Instance { get; private set; }

    public PlayerData data;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            data = new PlayerData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // get; set; methods
    public ulong Id
    {
        get { return data.Id; }
        set { data.Id = value; UpdateInterface(); }
    }

    public string Name
    {
        get { return data.Name; }
        set { data.Name = value; UpdateInterface(); }
    }

    public Color Color
    {
        get { return data.Color; }
        set { data.Color = value; UpdateInterface(); }
    }

    public bool Ready
    {
        get { return data.Ready; }
        set { data.Ready = value; UpdateInterface(); }
    }

    public int Avatar
    {
        get { return data.Avatar; }
        set { data.Avatar = value; UpdateInterface(); }
    }
    // ~get; set; methods

    public void Fetch() { UpdateInterface(); }

    private void UpdateInterface()
    {
        FindObjectOfType<LobbyOrchestrator>().UpdatePlayer(data);
    }
}
