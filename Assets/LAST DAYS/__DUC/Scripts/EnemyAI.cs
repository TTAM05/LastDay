using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player;

    private Node topNode;

    void Start()
    {
        Node attackRange =
            new IsPlayerInAttackRange(
                transform,
                player,
                2f);

        Node attack =
            new AttackNode(
                GetComponent<EnemyCombat>());

        Sequence attackSequence =
            new Sequence(new List<Node>
            {
                attackRange,
                attack
            });

        Node canSee =
            new IsPlayerInAttackRange(
                transform,
                player,
                10f);

        Node chase =
            new ChaseNode(
                          GetComponent<NavMeshAgent>(),
                          player,
                          GetComponent<Animator>());

        Sequence chaseSequence =
            new Sequence(new List<Node>
            {
                canSee,
                chase
            });

        Node patrol =
            new PatrolNode(
                            GetComponent<NavMeshAgent>(),
                            transform,
                            GetComponent<Animator>());

        topNode =
            new Selector(new List<Node>
            {
                attackSequence,
                chaseSequence,
                patrol
            });
    }

    void Update()
    {
        topNode.Evaluate();
    }
}
