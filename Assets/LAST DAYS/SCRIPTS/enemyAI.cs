using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyState
    {
        Patrol,
        Chase,
        Attack
    }

    [Header("References")]
    public Transform player;
    public NavMeshAgent agent;
    public Animator animator;

    [Header("Patrol")]
    public float patrolWaitTime = 2f;

    private Transform patrolCenter;
    private PatrolCenter patrolCenterData;

    private Vector3 patrolPoint;

    [Header("Detection")]
    public float detectionRange = 15f;
    public float loseRange = 20f;

    [Header("Attack")]
    public float attackRange = 2f;
    public float attackCooldown = 2f;

    [Header("Movement")]
    public float walkSpeed = 2f;
    public float chaseSpeed = 4f;

    private EnemyState currentState;

    private float waitTimer;
    private float attackTimer;

    private bool waiting;

    void Start()
    {

        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (player == null)
        {
            GameObject playerObj =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObj != null)
                player = playerObj.transform;
        }

        FindClosestPatrolCenter();
        SetNewPatrolPoint();
        

        currentState = EnemyState.Patrol;
    }

    void Update()
    {
        if (player == null) return;

        attackTimer += Time.deltaTime;

        float distance =
            Vector3.Distance(transform.position, player.position);

        UpdateState(distance);

        switch (currentState)
        {
            case EnemyState.Patrol:
                Patrol();
                break;

            case EnemyState.Chase:
                Chase();
                break;

            case EnemyState.Attack:
                Attack();
                break;
        }

        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    void UpdateState(float distance)
    {
        switch (currentState)
        {
            case EnemyState.Patrol:

                if (distance <= detectionRange)
                {
                    currentState = EnemyState.Chase;
                }

                break;

            case EnemyState.Chase:

                if (distance <= attackRange)
                {
                    currentState = EnemyState.Attack;
                }
                else if (distance >= loseRange)
                {
                    currentState = EnemyState.Patrol;

                    SetNewPatrolPoint();
                }

                break;

            case EnemyState.Attack:

                if (distance > attackRange)
                {
                    currentState = EnemyState.Chase;
                }

                break;
        }
    }

    void FindClosestPatrolCenter()
    {
        GameObject[] centers =
            GameObject.FindGameObjectsWithTag("PatrolCenter");

        float closestDistance = Mathf.Infinity;

        foreach (GameObject center in centers)
        {
            float distance = Vector3.Distance(
                transform.position,
                center.transform.position
            );

            if (distance < closestDistance)
            {
                closestDistance = distance;

                patrolCenter = center.transform;

                patrolCenterData =
                    center.GetComponent<PatrolCenter>();
            }
        }
    }

    void Patrol()
    {
        agent.isStopped = false;
        agent.speed = walkSpeed;

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

        agent.SetDestination(patrolPoint);

        if (!agent.pathPending &&
            agent.remainingDistance <= 0.5f)
        {
            waiting = true;
            waitTimer = 0f;
        }
    }

    void Chase()
    {
        agent.isStopped = false;
        agent.speed = chaseSpeed;

        agent.SetDestination(player.position);

        RotateToPlayer();
    }

    void Attack()
    {
        agent.isStopped = true;

        RotateToPlayer();

        if (attackTimer < attackCooldown)
            return;

        animator.SetTrigger("Attack");

        attackTimer = 0f;
    }

    void RotateToPlayer()
    {
        Vector3 dir =
            player.position - transform.position;

        dir.y = 0;

        Quaternion rot =
            Quaternion.LookRotation(dir);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            rot,
            Time.deltaTime * 5f
        );
    }

    void SetNewPatrolPoint()
    {
        if (patrolCenter == null ||
            patrolCenterData == null)
            return;

        float radius = patrolCenterData.patrolRadius;

        Vector2 randomCircle =
            Random.insideUnitCircle * radius;

        Vector3 randomPos = new Vector3(
            patrolCenter.position.x + randomCircle.x,
            patrolCenter.position.y,
            patrolCenter.position.z + randomCircle.y
        );

        NavMeshHit hit;

        if (NavMesh.SamplePosition(
            randomPos,
            out hit,
            radius,
            NavMesh.AllAreas))
        {
            patrolPoint = hit.position;
        }
    }

    void OnDrawGizmosSelected()
    {
        // Detection Range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(
            transform.position,
            detectionRange
        );

        // Attack Range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            transform.position,
            attackRange
        );

        // Lose Range
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(
            transform.position,
            loseRange
        );

    }
}