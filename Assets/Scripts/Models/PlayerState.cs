using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerState : INetworkSerializable
{
    public delegate void OnValueChanged();
    public event OnValueChanged OnValueChange;

    // Player Data
    private ulong id = 0;
    private string name = "Unknown";
    private Color color = Color.white;

    // Player State
    private int health = 100;
    private int score = 0;
    private int kills = 0;
    private int deaths = 0;
    private int totalDamage = 0;
    private int totalHealing = 0;
    private int totalShots = 0;

    // Player Location
    private Vector3 position = Vector3.zero;
    private Quaternion rotation = Quaternion.identity;
    private Vector3 scale = Vector3.one;

    // Player Movement
    private Vector3 velocity = Vector3.zero;
    private Vector3 acceleration = Vector3.zero;
    private Vector3 angularVelocity = Vector3.zero;
    private Vector3 angularAcceleration = Vector3.zero;

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

        serializer.SerializeValue(ref health);
        serializer.SerializeValue(ref score);
        serializer.SerializeValue(ref kills);
        serializer.SerializeValue(ref deaths);
        serializer.SerializeValue(ref totalDamage);
        serializer.SerializeValue(ref totalHealing);
        serializer.SerializeValue(ref totalShots);

        serializer.SerializeValue(ref position);
        serializer.SerializeValue(ref rotation);
        serializer.SerializeValue(ref scale);

        serializer.SerializeValue(ref velocity);
        serializer.SerializeValue(ref acceleration);
        serializer.SerializeValue(ref angularVelocity);
        serializer.SerializeValue(ref angularAcceleration);

        serializer.SerializeValue(ref animation);
        serializer.SerializeValue(ref spriteRenderer);
        serializer.SerializeValue(ref animator);
    }
    // ~INetworkSerializable

    // Constructors
    public PlayerState() { }
    public PlayerState(PlayerData data)
    {
        id = data.Id;
        name = data.Name;
        color = data.Color;

        animator = data.Animator;
        animation = data.Animation;
        spriteRenderer = data.SpriteRenderer;
    }
    // ~Constructors

    // set conditions
    public bool IsAlive { get => health > 0; }
    public bool IsPaused { get => throw new NotImplementedException("This method is not implemented."); }
    public bool IsRunning { get => throw new NotImplementedException("This method is not implemented."); }
    public bool IsJumping { get => throw new NotImplementedException("This method is not implemented."); }
    public bool IsFalling { get => throw new NotImplementedException("This method is not implemented."); }
    public bool IsCrouching { get => throw new NotImplementedException("This method is not implemented."); }
    public bool IsClimbing { get => throw new NotImplementedException("This method is not implemented."); }
    public bool IsAttacking { get => throw new NotImplementedException("This method is not implemented."); }
    // ~set conditions

    // get; set; methods, and OnValueChange() calls
    public ulong Id { get => id; set { id = value; OnValueChange?.Invoke(); } }
    public string Name { get => name; set { name = value; OnValueChange?.Invoke(); } }
    public Color Color { get => color; set { color = value; OnValueChange?.Invoke(); } }
    public int Health { get => health; set { health = value; OnValueChange?.Invoke(); } }
    public int Score { get => score; set { score = value; OnValueChange?.Invoke(); } }
    public int Kills { get => kills; set { kills = value; OnValueChange?.Invoke(); } }
    public int Deaths { get => deaths; set { deaths = value; OnValueChange?.Invoke(); } }
    public int TotalDamage { get => totalDamage; set { totalDamage = value; OnValueChange?.Invoke(); } }
    public int TotalHealing { get => totalHealing; set { totalHealing = value; OnValueChange?.Invoke(); } }
    public int TotalShots { get => totalShots; set { totalShots = value; OnValueChange?.Invoke(); } }
    public Vector3 Position { get => position; set { position = value; OnValueChange?.Invoke(); } }
    public Quaternion Rotation { get => rotation; set { rotation = value; OnValueChange?.Invoke(); } }
    public Vector3 Scale { get => scale; set { scale = value; OnValueChange?.Invoke(); } }
    public Vector3 Velocity { get => velocity; set { velocity = value; OnValueChange?.Invoke(); } }
    public Vector3 Acceleration { get => acceleration; set { acceleration = value; OnValueChange?.Invoke(); } }
    public Vector3 AngularVelocity { get => angularVelocity; set { angularVelocity = value; OnValueChange?.Invoke(); } }
    public Vector3 AngularAcceleration { get => angularAcceleration; set { angularAcceleration = value; OnValueChange?.Invoke(); } }
    public int Animation { get => animation; set { animation = value; OnValueChange?.Invoke(); } }
    public int SpriteRenderer { get => spriteRenderer; set { spriteRenderer = value; OnValueChange?.Invoke(); } }
    public int Animator { get => animator; set { animator = value; OnValueChange?.Invoke(); } }
    // ~get; set; methods, and OnValueChange() calls

    public override string ToString()
    {
        return $"PlayerState: id={id}, name={name}, color={color}, health={health}, score={score}, kills={kills}, deaths={deaths}, totalDamage={totalDamage}, totalHealing={totalHealing}, totalShots={totalShots}, animation={animation}, spriteRenderer={spriteRenderer}, animator={animator}";
    }
}
