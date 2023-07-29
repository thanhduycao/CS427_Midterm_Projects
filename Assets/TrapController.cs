using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapController : MonoBehaviour
{
    [Header("Trap Name")]
    [SerializeField] public string Sname;
    [Header("Trap Damage")]
    [SerializeField] public int damage = 20;

    public int getDamage()
    {
        Debug.Log("Trap Damage: " + damage);
        return damage;
    }

    public void setDame(int damage)
    {
        this.damage = damage;
    }
}
