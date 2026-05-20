using UnityEngine;

public class IsPlayerInAttackRange : Node
{
    private Transform enemy;
    private Transform player;
    private float attackRange;

    public IsPlayerInAttackRange(
        Transform enemy,
        Transform player,
        float attackRange)
    {
        this.enemy = enemy;
        this.player = player;
        this.attackRange = attackRange;
    }

    public override NodeState Evaluate()
    {
        if (player == null)
            return NodeState.Failure;

        float distance =
            Vector3.Distance(
                enemy.position,
                player.position);

        if (distance <= attackRange)
        {
            return NodeState.Success;
        }

        return NodeState.Failure;
    }
}