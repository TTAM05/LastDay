// using UnityEngine;

// public class AttackNode : Node
// {
//     private EnemyCombat combat;

//     public AttackNode(EnemyCombat combat)
//     {
//         this.combat = combat;
//     }

//     public override NodeState Evaluate()
//     {
//         if (combat.CanAttack())
//         {
//             combat.Attack();

//             return NodeState.Running;
//         }

//         return NodeState.Failure;
//     }
// }