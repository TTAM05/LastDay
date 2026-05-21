using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    public ZombieData zombieData;
    public float currentHealth;
    bool isDead = false;
    private Animator anim;

    //Sound
    public AudioSource audioSource;

    void Start()
    {
        anim = GetComponent<Animator>();
        if (zombieData != null)
        {
            currentHealth = zombieData.maxHealth;
        }
        else
        {
            Debug.LogError("ZombieData not assigned on " + gameObject.name);
            currentHealth = 100f; // default value
        }
    }

    public void TakeDamage(float damage, bool isHeadshot)
    {
        if(isDead)
            return;

            
        currentHealth -= damage;

        Debug.Log(gameObject.name + 
            " took " + damage + 
            " damage. Remaining health: " + currentHealth);

        if (currentHealth <= 0)
        {   
            isDead = true;
            Die(isHeadshot);
        }
    }

    void Die(bool isHeadshot)
    {
        // tắt AI
        EnemyAI ai = GetComponent<EnemyAI>();

        if (ai != null)
        {
            ai.enabled = false;
        }

        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.enabled = false;
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.isKinematic = true;
        }

        // tắt collider nếu muốn
        foreach (Collider col in GetComponentsInChildren<Collider>())
        {
            col.enabled = false;
        }


        if (isHeadshot)
        {
            if (anim != null)
            {
                anim.SetTrigger("HeadShot");
            }

            if (audioSource != null)
            {
                audioSource.Stop();
                audioSource.PlayOneShot(zombieData.DieSound);
            }

            Debug.Log("Headshot!");
        }
        else
        {
            if (anim != null)
            {
                anim.SetTrigger("Die");
            }

            if (audioSource != null)
            {   
                audioSource.Stop();
                audioSource.PlayOneShot(zombieData.DieSound);
            }

            Debug.Log(gameObject.name + " has died.");
        }

        Destroy(gameObject, 5f);
    }
}