using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [Header("Random Move")]
    public float moveRange = 10f;
    public float waitTime = 2f;

    private NavMeshAgent agent;

    private Vector3 startPosition;
    private float timer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        startPosition = transform.position;

        MoveToRandomPoint();
    }

    void Update()
    {
        // Nếu enemy đã tới điểm
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            timer += Time.deltaTime;

            // Chờ rồi đi tiếp
            if (timer >= waitTime)
            {
                MoveToRandomPoint();
                timer = 0f;
            }
        }
    }

    void MoveToRandomPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * moveRange;

        randomDirection += startPosition;

        NavMeshHit hit;

        // Tìm điểm hợp lệ trên NavMesh
        if (NavMesh.SamplePosition(randomDirection, out hit, moveRange, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }
}