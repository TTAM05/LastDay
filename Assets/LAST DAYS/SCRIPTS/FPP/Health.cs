using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections;

public class Health : MonoBehaviour
{
    public CharData charData;
    [Header("Health")]
    public float currentHealth;
    private bool isDead= false;

    [Header("Damage Cooldown")]
    public float damageCooldown = 1f; // thời gian miễn sát thương
    private float lastDamageTime;

    [Header("UI")]
    public Image healthBarFill; 

    [Header("Audio")]
    public AudioSource audioSource;

    [Header("GunSystem")]
    public GunSystem[] gunSystem;
    public AimSystem[] aimSystem;
    public MacheteSystem[] macheteSystem;

    void Awake()
    {
        currentHealth = charData.maxHealth;
        lastDamageTime = -999f;

        UpdateHealthBar();
    }

    //NHẬN DAMAGE (có cooldown)
    public void TakeDamage(float damage)
    {   
        if(isDead) return;
        if (Time.time < lastDamageTime + damageCooldown)
            return; // đang trong thời gian bất tử

        lastDamageTime = Time.time;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, charData.maxHealth);

        // play hit sound
        if (charData.hitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(charData.hitSound);
        }



        UpdateHealthBar();


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

        UpdateHealthBar();
    }

    void Die()
    {   
        if (isDead) return;
        isDead = true;

        Debug.Log("Dead");
        // play death sound
        if (charData.deathSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(charData.deathSound);
        }

        FPSController fps = GetComponentInParent<FPSController>();
        if (fps != null)
            fps.Die();

        // Freeze game after short delay to allow sound to play
        StartCoroutine(FreezeAfterDelay(2f));
        
    }

    //va chạm với zombie
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyHand"))
        {
            // Leo lên Parent để lấy ZombieData
            EnemyAI enemyHand = other.GetComponentInParent<EnemyAI>();

            if (enemyHand != null)
                GetComponent<Health>().TakeDamage(enemyHand.zombieData.damage);

                Debug.Log("Player hit by enemy hand");
        }
    }

    void UpdateHealthBar()
    {
        if (healthBarFill != null)
            // fillAmount từ 0 đến 1
            healthBarFill.fillAmount = currentHealth / charData.maxHealth;
    }

    
    IEnumerator FreezeAfterDelay(float delay)
    {
        gunSystem = GetComponentsInChildren<GunSystem>(true);
        aimSystem = GetComponentsInChildren<AimSystem>(true);
        macheteSystem = GetComponentsInChildren<MacheteSystem>(true);

        foreach (var gun in gunSystem)
        {
            if (gun != null)
                gun.enabled = false;
        }

        foreach (var aim in aimSystem)
        {
            if (aim != null)
                aim.enabled = false;
        }

        foreach (var machete in macheteSystem)
        {
            if (machete != null)
                machete.enabled = false;
        }

        yield return new WaitForSecondsRealtime(delay);

        Debug.Log("Game Over");
        Time.timeScale = 0f;
    }
}