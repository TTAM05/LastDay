using UnityEngine;
using UnityEngine.AI;

public class ChaseNode : Node
{
    private NavMeshAgent agent;
    private Transform player;
    private Animator animator;

    public ChaseNode(
        NavMeshAgent agent,
        Transform player,
        Animator animator)
    {
        this.agent = agent;
        this.player = player;
        this.animator = animator;
    }

    public override NodeState Evaluate()
    {
        agent.isStopped = false;

        agent.SetDestination(player.position);

        animator.SetFloat(
            "Speed",
            agent.velocity.magnitude);

        return NodeState.Running;
    }
}