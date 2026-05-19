using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Animator")]
    public Animator animator;

    [Header("Death")]
    public bool isDead;

    private void Start()
    {
        currentHealth = maxHealth;

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    // private void OnTriggerEnter(Collider other)
    // {
    //     // Kiểm tra tag Bullet
    //     if (other.CompareTag("Bullet"))
    //     {
    //         Bullet bullet = other.GetComponent<Bullet>();

    //         // Nếu có script Bullet thì lấy damage
    //         if (bullet != null)
    //         {
    //             TakeDamage(bullet.damage);
    //         }
    //         else
    //         {
    //             // Damage mặc định
    //             TakeDamage(10f);
    //         }

    //         Destroy(other.gameObject);
    //     }
    // }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        Debug.Log(gameObject.name + " HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        // Trigger animation Die
        animator.SetTrigger("Die");

        // Tắt EnemyAI nếu có
        EnemyAI ai = GetComponent<EnemyAI>();
        if (ai != null)
        {
            ai.enabled = false;
        }

        // Tắt NavMeshAgent nếu có
        UnityEngine.AI.NavMeshAgent agent =
            GetComponent<UnityEngine.AI.NavMeshAgent>();

        if (agent != null)
        {
            agent.enabled = false;
        }

        // Destroy enemy sau vài giây
        Destroy(gameObject, 5f);
    }
}