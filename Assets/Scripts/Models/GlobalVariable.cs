using UnityEngine;

public class GlobalVariable : MonoBehaviour
{
    public static GlobalVariable Instance { get; private set; }

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
}
