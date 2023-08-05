using Unity.Netcode;
using UnityEngine;

public class PlayerData : INetworkSerializable
{
    public ulong id = 0;
    public string name = NVJOBNameGen.Uppercase(NVJOBNameGen.GiveAName(7));
    public Color color = Color.white;
    public bool ready = false;

    // INetworkSerializable
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref name);
        serializer.SerializeValue(ref color);
        serializer.SerializeValue(ref ready);
    }
    // ~INetworkSerializable

    public PlayerData(ulong _id = 0, string _name = "", Color _color = new Color(), bool _ready = false)
    {
        id = _id;
        name = _name;
        color = _color;
        ready = _ready;
    }

    public PlayerData() { }
}