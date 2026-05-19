using UnityEngine;

public class PatrolCenter : MonoBehaviour
{
    public float patrolRadius = 15f;

    //gizmos
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, patrolRadius);
    }
}

