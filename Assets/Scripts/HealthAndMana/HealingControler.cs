using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingControler : MonoBehaviour
{
    [Header("Healing Name")]
    [SerializeField] public string Sname;
    [Header("Healing value")]
    [SerializeField] public int healing = 20;

    public int getHealing()
    {
        return healing;
    }

    public void setHealing(int healing)
    {
        this.healing = healing;
    }
}
