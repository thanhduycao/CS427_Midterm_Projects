using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReduceMana : MonoBehaviour
{
    public int maxMana = 100;

    public int currentMana;

    public ManaBar manaBar;

    public GameObject player1, player2;
    private void Start()
    {
        currentMana = maxMana;
        manaBar.setMaxMana(maxMana);
    }

    public void spendMana(int spend)
    {
        currentMana -= spend;
        manaBar.setMana(currentMana);
    }

   public void setCurrentMana(int mana)
    {
        currentMana = mana;
        manaBar.setMana(mana);
    }
}
