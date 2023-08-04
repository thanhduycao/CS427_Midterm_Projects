using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrenPlayerData : MonoBehaviour
{
    public static CurrenPlayerData Instance { get; private set; }

    public LobbyPlayerData currentPlayerData;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            currentPlayerData = new LobbyPlayerData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetName(string name)
    {
        currentPlayerData.name = name;
    }

    public void SetColor(Color color)
    {
        currentPlayerData.color = color;
    }

    public void SetReady(bool ready)
    {
        currentPlayerData.ready = ready;
    }

    public void SetPlayerData(LobbyPlayerData data)
    {
        currentPlayerData = data;
    }

    public string GetName()
    {
        return currentPlayerData.name;
    }

    public Color GetColor()
    {
        return currentPlayerData.color;
    }

    public bool GetReady()
    {
        return currentPlayerData.ready;
    }
}
