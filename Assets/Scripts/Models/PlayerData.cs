using Unity.Netcode;
using UnityEngine;

public class PlayerData : INetworkSerializable
{
    // Player Data
    private ulong id = 0;
    private string name = "Unknown";
    private Color color = Color.white;
    private bool ready = false;
    private bool isLocalPlayer = true;

    // Player Assets
    private int animator = 0;
    private int animation = 0;
    private int spriteRenderer = 0;

    // INetworkSerializable
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref id);
        serializer.SerializeValue(ref name);
        serializer.SerializeValue(ref color);
        serializer.SerializeValue(ref ready);
        serializer.SerializeValue(ref isLocalPlayer);

        serializer.SerializeValue(ref animation);
        serializer.SerializeValue(ref spriteRenderer);
        serializer.SerializeValue(ref animator);
    }
    // ~INetworkSerializable

    // Constructors
    public PlayerData() { }
    public PlayerData(ulong id = 0, string name = "", bool ready = false, bool isLocalPlayer = false, int animation = 0, int spriteRenderer = 0, int animator = 0)
    {
        this.id = id;
        this.name = name;
        // this.color = color;
        this.ready = ready;
        this.isLocalPlayer = isLocalPlayer;
        this.animation = animation;
        this.spriteRenderer = spriteRenderer;
        this.animator = animator;
    }
    public PlayerData(ulong id) { this.id = id; }
    public PlayerData(ulong id, Color color) { this.id = id; this.color = color; }
    public PlayerData(string name) { this.name = name; }
    public PlayerData(Color color) { this.color = color; }
    public PlayerData(bool ready) { this.ready = ready; }
    // ~Constructors

    public override string ToString()
    {
        return $"PlayerData: id={id}, name={name}, color={color}, ready={ready}, isLocalPlayer={isLocalPlayer}, animation={animation}, spriteRenderer={spriteRenderer}, animator={animator}";
    }

    // set conditions
    public bool IsReady() { return ready; }
    public bool IsLocal() { return isLocalPlayer; }
    // ~set conditions

    // set get; set methods
    public ulong Id
    {
        get { return id; }
        set { id = value; }
    }
    public string Name
    {
        get { return name; }
        set { name = value; }
    }
    public Color Color
    {
        get { return color; }
        set { color = value; }
    }
    public bool Ready
    {
        get { return ready; }
        set { ready = value; }
    }
    public bool IsLocalPlayer
    {
        get { return isLocalPlayer; }
        set { isLocalPlayer = value; }
    }
    public int Animation
    {
        get { return animation; }
        set { animation = value; }
    }
    public int SpriteRenderer
    {
        get { return spriteRenderer; }
        set { spriteRenderer = value; }
    }
    public int Animator
    {
        get { return animator; }
        set { animator = value; }
    }
    // ~set get; set methods
}