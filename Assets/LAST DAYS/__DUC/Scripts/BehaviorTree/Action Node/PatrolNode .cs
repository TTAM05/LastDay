using UnityEngine;
using UnityEngine.AI;

public class PatrolNode : Node
{
    private NavMeshAgent agent;
    private Transform enemy;
    private Animator animator;

    private Vector3 patrolPoint;
    private float patrolRange = 10f;

    public PatrolNode(
        NavMeshAgent agent,
        Transform enemy,
        Animator animator)
    {
        this.agent = agent;
        this.enemy = enemy;
        this.animator = animator;

        SetRandomDestination();
    }

    public override NodeState Evaluate()
    {
        animator.SetFloat(
            "Speed",
            agent.velocity.magnitude);

        if (!agent.pathPending &&
            agent.remainingDistance < 0.5f)
        {
            SetRandomDestination();
        }

        return NodeState.Running;
    }

    private void SetRandomDestination()
    {
        Vector3 randomPoint =
            enemy.position +
            Random.insideUnitSphere * patrolRange;

        randomPoint.y = enemy.position.y;

        NavMeshHit hit;

        if (NavMesh.SamplePosition(
            randomPoint,
            out hit,
            patrolRange,
            NavMesh.AllAreas))
        {
            patrolPoint = hit.position;

            agent.SetDestination(patrolPoint);
        }
    }
}