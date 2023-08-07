using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapController : MonoBehaviour
{
    [SerializeField] private int damage = 10;

    public int Damage { get => damage; set => damage = value; }
}
