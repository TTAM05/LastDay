using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Damage Cooldown")]
    public float damageCooldown = 1f; // thời gian miễn sát thương
    private float lastDamageTime;

    public event Action<float> OnHealthChanged;
    public event Action OnDie;

    void Awake()
    {
        currentHealth = maxHealth;
        lastDamageTime = -999f;
    }

    //NHẬN DAMAGE (có cooldown)
    public void TakeDamage(float damage)
    {
        if (Time.time < lastDamageTime + damageCooldown)
            return; // đang trong thời gian bất tử

        lastDamageTime = Time.time;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnHealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // HỒI MÁU
    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnHealthChanged?.Invoke(currentHealth);
    }

    void Die()
    {
        OnDie?.Invoke();
        Debug.Log("Dead");
    }
}