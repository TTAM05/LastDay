using UnityEngine;
using UnityEngine.AI;

public class EnemyCombat : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public int damage = 10;

    [Header("References")]
    public Transform player;

    private float lastAttackTime;

    private Animator animator;
    private NavMeshAgent agent;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    public bool CanAttack()
    {
        if (player == null)
            return false;

        float distance =
            Vector3.Distance(
                transform.position,
                player.position);

        return distance <= attackRange;
    }

    public bool Attack()
    {
        if (player == null)
            return false;

        if (Time.time < lastAttackTime + attackCooldown)
            return false;

        float distance =
            Vector3.Distance(
                transform.position,
                player.position);

        if (distance > attackRange)
            return false;

        lastAttackTime = Time.time;

        agent.isStopped = true;

        RotateToPlayer();

        animator.SetTrigger("Attack");

        Debug.Log("Enemy Attack");

        return true;
    }

    public void ResumeMovement()
    {
        agent.isStopped = false;
    }

    private void RotateToPlayer()
    {
        Vector3 direction =
            (player.position - transform.position).normalized;

        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation =
                Quaternion.LookRotation(direction);

            transform.rotation =
                Quaternion.Slerp(
                    transform.rotation,
                    lookRotation,
                    10f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            attackRange);
    }
}