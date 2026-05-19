using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    public float health = 100f;

    bool isDead = false;
    private Animator anim;

    //Sound
    public AudioSource audioSource;
    public AudioClip DieClip;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void TakeDamage(float damage, bool isHeadshot)
    {
        if(isDead)
            return;

            
        health -= damage;

        Debug.Log(gameObject.name + 
            " took " + damage + 
            " damage. Remaining health: " + health);

        if (health <= 0)
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
                audioSource.PlayOneShot(DieClip);
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
                audioSource.PlayOneShot(DieClip);
            }

            Debug.Log(gameObject.name + " has died.");
        }

        Destroy(gameObject, 5f);
    }
}