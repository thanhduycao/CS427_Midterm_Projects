using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVariable : MonoBehaviour
{
    public static GlobalVariable Instance { get; private set; }

    public int gameMode { get; set; } = 0; // 0: offline, 1: online
}
