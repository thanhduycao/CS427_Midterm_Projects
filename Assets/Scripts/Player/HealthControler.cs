using UnityEngine;

public class HealthControler : MonoBehaviour
{
    [SerializeField] private int currentHealth = 100;
    [SerializeField] private HealthBar healthBar;

    private readonly int maxHealth = 100;

    private void Start()
    {
        healthBar.SetMaxHealth(maxHealth);
    }

    private void FixedUpdate()
    {
        healthBar.SetHealth(currentHealth);
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
        healthBar.SetHealth(currentHealth);
    }

    public void TakeAllDamage()
    {
        currentHealth = 0;
        healthBar.SetHealth(currentHealth);
    }

    public void Health(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        healthBar.SetHealth(currentHealth);
    }

    public void HealthAll()
    {
        currentHealth = maxHealth;
        healthBar.SetHealth(currentHealth);
    }
}
