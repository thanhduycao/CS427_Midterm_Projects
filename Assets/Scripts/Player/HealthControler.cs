using UnityEngine;

public class HealthControler : MonoBehaviour
{
    [SerializeField] private int currentHealth = 100;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private TMPro.TMP_Text playerName;

    private readonly int maxHealth = 100;

    private void Start()
    {
        healthBar.SetMaxHealth(maxHealth);
    }

    private void FixedUpdate()
    {
        healthBar.SetHealth(currentHealth);

        //int flip = transform.localScale.x > 0 ? 1 : -1;
        //FlipPlayerName(flip);
        //healthBar.Flip(flip);
    }

    public void SetHealth(int health)
    {
        healthBar.SetHealth(health);
        currentHealth = health;
    }

    public int GetHealth()
    {
        return currentHealth;
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

    public void SetPlayerName(string name)
    {
        playerName.text = name;
    }

    public void SetColor(Color color)
    {
        playerName.color = color;
    }

    public void FlipPlayerName(int flip)
    {
        playerName.transform.localScale = new Vector3(flip * playerName.transform.localScale.x, playerName.transform.localScale.y, playerName.transform.localScale.z);
    }
}
