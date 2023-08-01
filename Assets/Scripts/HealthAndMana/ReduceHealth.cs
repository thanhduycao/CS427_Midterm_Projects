using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReduceHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public HealthBar healthBar;

    private void Start()
    {
        currentHealth = maxHealth;
        healthBar.setMaxHealth(maxHealth);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Trap _))
        {
            GetComponent<MovementNoMana>().OnHit(true);
            takeDamage(collision.GetComponent<TrapController>().getDamage());
        }
        else if (collision.TryGetComponent(out Heal _))
        { 
            heal(collision.GetComponent<HealingControler>().getHealing());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Trigger Exit");
        if (collision.TryGetComponent(out Trap _))
        {
            Debug.Log("Trigger Exit Trap");
            GetComponent<MovementNoMana>().OnHit(false);
        }
    }

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

    public void heal(int healing)
    {
        currentHealth += healing;
        healthBar.setHealth(currentHealth);
    }

    public void healAll()
    {
        currentHealth = maxHealth;
        healthBar.setHealth(currentHealth);
    }
}
