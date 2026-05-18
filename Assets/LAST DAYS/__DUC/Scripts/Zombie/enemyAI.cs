using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public NavMeshAgent agent;
    public Animator animator;

    [Header("Patrol")]
    public float patrolRadius = 10f;
    public float patrolWaitTime = 2f;

    [Header("Detection")]
    public float detectionRange = 15f;
    public float loseDistance = 20f;

    [Header("Attack")]
    public float attackRange = 2f;
    public float attackCooldown = 2f;

    [Header("Movement")]
    public float walkSpeed = 2f;
    public float chaseSpeed = 4f;

    private Vector3 startPosition;
    private Vector3 patrolPoint;

    private bool waiting;
    private bool chasing;
    private bool attacking;

    private float waitTimer;
    private float attackTimer;

    void Start()
    {
        startPosition = transform.position;

        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        SetNewPatrolPoint();
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        attackTimer += Time.deltaTime;

        // Detect Player
        if (distance <= detectionRange)
        {
            chasing = true;
        }

        // Lose Player
        if (distance >= loseDistance)
        {
            chasing = false;
            attacking = false;

            agent.speed = walkSpeed;

            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                SetNewPatrolPoint();
            }
        }

        // Chase
        if (chasing && !attacking)
        {
            ChasePlayer(distance);
        }
        else
        {
            Patrol();
        }

        // Animation Speed
        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    void Patrol()
    {
        if (waiting)
        {
            waitTimer += Time.deltaTime;

            if (waitTimer >= patrolWaitTime)
            {
                waiting = false;
                SetNewPatrolPoint();
            }

            return;
        }

        agent.speed = walkSpeed;
        agent.SetDestination(patrolPoint);

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            waiting = true;
            waitTimer = 0f;
        }
    }

    void ChasePlayer(float distance)
    {
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);

        transform.LookAt(new Vector3(
            player.position.x,
            transform.position.y,
            player.position.z
        ));

        // Attack
        if (distance <= attackRange)
        {
            Attack();
        }
    }

    void Attack()
    {
        if (attackTimer < attackCooldown) return;

        attacking = true;

        agent.SetDestination(transform.position);

        animator.SetTrigger("Attack");

        attackTimer = 0f;

        Invoke(nameof(StopAttack), 1f);
    }

    void StopAttack()
    {
        attacking = false;
    }

    void SetNewPatrolPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += startPosition;

        NavMeshHit hit;

        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, 1))
        {
            patrolPoint = hit.position;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, loseDistance);
    }
}