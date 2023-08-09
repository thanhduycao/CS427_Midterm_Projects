using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;

    private void Start()
    {
        slider.maxValue = 100;
        slider.value = 70;
    }

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    public void SetHealth(int health)
    {
        slider.value = health;
    }

    public void Flip(int flip)
    {
        slider.transform.localScale = new Vector3(flip * slider.transform.localScale.x, slider.transform.localScale.y, slider.transform.localScale.z);
    }
}