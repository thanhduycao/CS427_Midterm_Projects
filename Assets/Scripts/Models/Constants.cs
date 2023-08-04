using System.Collections.Generic;
using UnityEngine;

public class Constants {
    public const string JoinKey = "j";
    public const string RoundKey = "r";
    public const string ColorKey = "c";

    public static readonly List<string> Rounds = new() { "Round1", "Round2" };
    public static readonly List<Color> Colors = new() { Color.red, Color.blue, Color.green, Color.yellow, Color.magenta, Color.cyan };
}