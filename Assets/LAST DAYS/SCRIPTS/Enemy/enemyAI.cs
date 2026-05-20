using System.Collections;
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
    public GameObject BloodScreenObj;
    private GameObject InstantiatedObj;
    private Coroutine bloodCoroutine;
    public ZombieData zombieData;

    [Header("Patrol")]
    public float patrolWaitTime = 2f;

    private Transform patrolCenter;
    private PatrolCenter patrolCenterData;
    private int currentWaypointIndex = 0;
    private Vector3 patrolPoint;

    [Header("Detection")]
    public float detectionRange = 15f;
    public float loseRange = 20f;

    [Header("Attack")]
    public float attackRange = 2f;
    public float attackCooldown = 2f;


    [Header("Audio")]
    public AudioSource audioSource;

    private EnemyState currentState;

    private float waitTimer;
    private float attackTimer;

    private bool waiting;

    private bool isInitialized = false;

    void Start()
    {

        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        agent.updateRotation = false;

        if (player == null)
        {
            GameObject playerObj =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObj != null)
                player = playerObj.transform;
        }

        FindClosestPatrolCenter();
        SetNewPatrolPoint();
        SetClosestWaypointAsStart();

        StartCoroutine(InitWhenReady());
        

        currentState = EnemyState.Patrol;
    }

    void Update()
    {
        if (!isInitialized ||player == null) return;

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


    void SetClosestWaypointAsStart()
    {
        if (patrolCenterData == null ||
            patrolCenterData.waypoints == null ||
            patrolCenterData.waypoints.Length == 0)
            return;

        float closest = Mathf.Infinity;

        for (int i = 0; i < patrolCenterData.waypoints.Length; i++)
        {
            if (patrolCenterData.waypoints[i] == null) continue;

            float dist = Vector3.Distance(
                transform.position,
                patrolCenterData.waypoints[i].position
            );

            if (dist < closest)
            {
                closest = dist;
                currentWaypointIndex = i;
            }
        }

        patrolPoint = patrolCenterData.waypoints[currentWaypointIndex].position;
    }

    void RotateToMovement()
    {
        Vector3 dir = agent.velocity;
        dir.y = 0;

        // Fallback: nếu velocity quá nhỏ (vừa bắt đầu / đang chờ), xoay về hướng patrol point
        if (dir.sqrMagnitude < 0.01f)
        {
            dir = patrolPoint - transform.position;
            dir.y = 0;

            if (dir.sqrMagnitude < 0.01f)
                return;
        }

        Quaternion rot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            rot,
            Time.deltaTime * 10f
        );
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

    private IEnumerator InitWhenReady()
    {
        // Chờ đến khi agent thực sự nằm trên NavMesh
        yield return new WaitUntil(() => agent.isOnNavMesh);

        agent.updateRotation = false;
        agent.speed = zombieData.Walkspeed;
        agent.acceleration = 15f;

        FindClosestPatrolCenter();
        SetClosestWaypointAsStart();

        currentState = EnemyState.Patrol;
        isInitialized = true;
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
        if (agent == null || !agent.isOnNavMesh)
            return;

        PlayAudio(zombieData.PatrolSound);

        agent.isStopped = false;
        agent.speed = zombieData.Walkspeed;

        // ✅ Luôn gọi rotate kể cả khi đang chờ
        RotateToMovement();

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
        if(agent == null || !agent.isOnNavMesh)
            return;

        PlayAudio(zombieData.ChaseSound);

        agent.isStopped = false;
        agent.speed = zombieData.Runspeed;

        agent.SetDestination(player.position);

        RotateToMovement();
    }

    void Attack()
    {
        if(agent == null || !agent.isOnNavMesh)
            return;

        // audioSource.PlayOneShot(attackClip);
        agent.isStopped = true;

        RotateToPlayer();

        if (attackTimer < attackCooldown)
            return;

        animator.SetTrigger("Attack");

        //hiện bloodScreen
        if (bloodCoroutine != null)
        {
            StopCoroutine(bloodCoroutine);
        }

        bloodCoroutine = StartCoroutine(ActiveBloodScreen());

        attackTimer = 0f;
    }

    void RotateToPlayer()
    {
        Vector3 dir =
            player.position - transform.position;

        dir.y = 0;

        // tránh vector zero
        if (dir.sqrMagnitude < 0.001f)
            return;

        Quaternion rot =
            Quaternion.LookRotation(dir);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            rot,
            Time.deltaTime * 10f
        );
    }

    void SetNewPatrolPoint()
    {
        if (patrolCenterData == null ||
            patrolCenterData.waypoints == null ||
            patrolCenterData.waypoints.Length == 0)
            return;

        // Random index, tránh trùng điểm cũ
        int newIndex;

        do
        {
            newIndex = Random.Range(0, patrolCenterData.waypoints.Length);
        }
        while (newIndex == currentWaypointIndex &&
            patrolCenterData.waypoints.Length > 1);

        currentWaypointIndex = newIndex;

        Transform wp = patrolCenterData.waypoints[currentWaypointIndex];

        if (wp != null)
            patrolPoint = wp.position;
    }

    void PlayAudio(AudioClip clip)
    {
        if (audioSource == null || clip == null)
            return;

        if (audioSource.clip == clip && audioSource.isPlaying)
            return;

        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }

    void InstantiatedObject()
    {
        if (InstantiatedObj != null) return;

        InstantiatedObj = Instantiate(BloodScreenObj);
    }

    void DeleteObject()
    {
        if(InstantiatedObj != null)
        {
            Destroy(InstantiatedObj);
            InstantiatedObj = null;
        }
    }

    private IEnumerator ActiveBloodScreen()
    {
        InstantiatedObject();
        yield return new WaitForSeconds(attackCooldown);
        DeleteObject();
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