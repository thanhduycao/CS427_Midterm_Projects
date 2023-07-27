using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReduceHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public HealthBar healthBar;

    public GameObject player1, player2;
    private void Start()
    {
        currentHealth = maxHealth;
        healthBar.setMaxHealth(maxHealth);
    }

    /*private void OnCollisionEnter2D(Collision2D collision)
    {
        if (CameraController2.currentPlayer == 1)
        {
            if (collision.transform.tag == "ball")
            {
                takeDamage(20);
            }
        }
    }*/

    public void takeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.setHealth(currentHealth);
    }

    public void takeAllDamage()
    {
        currentHealth = 0;
        healthBar.setHealth(currentHealth);
    }
}
