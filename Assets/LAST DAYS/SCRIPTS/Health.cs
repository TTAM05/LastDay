using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    public CharData charData;
    [Header("Health")]
    public float currentHealth;

    [Header("Damage Cooldown")]
    public float damageCooldown = 1f; // thời gian miễn sát thương
    private float lastDamageTime;

    void Awake()
    {
        currentHealth = charData.maxHealth;
        lastDamageTime = -999f;
    }

    //NHẬN DAMAGE (có cooldown)
    public void TakeDamage(float damage)
    {
        if (Time.time < lastDamageTime + damageCooldown)
            return; // đang trong thời gian bất tử

        lastDamageTime = Time.time;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, charData.maxHealth);


        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // HỒI MÁU
    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, charData.maxHealth);
    }

    void Die()
    {
        Debug.Log("Dead");
    }
}