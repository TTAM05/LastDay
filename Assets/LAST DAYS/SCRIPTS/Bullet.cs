using UnityEngine;

public class Bullet : MonoBehaviour
{
    // thời gian tự hủy
    public float lifeTime = 3f;
    // sát thương của viên đạn
    public float damage ;
    

    void Start()
    {
        // tự hủy
        Destroy(gameObject, lifeTime);
    }

    // void OnTriggerEnter(Collider other)
    // {
    //     Debug.Log("Hit: " + other.name);

    //     // HEADSHOT
    //     if (other.CompareTag("EnemyHead"))
    //     {
    //         EnemyHealth enemy =
    //             other.GetComponentInParent<EnemyHealth>();

    //         if (enemy != null)
    //         {
    //             enemy.TakeDamage(damage * 2);
    //         }

    //         Destroy(gameObject);
    //         return;
    //     }

    //     // BODY
    //     if (other.CompareTag("Enemy"))
    //     {
    //         EnemyHealth enemy =
    //             other.GetComponent<EnemyHealth>();

    //         if (enemy != null)
    //         {
    //             enemy.TakeDamage(damage);
    //         }

    //         Destroy(gameObject);
    //         return;
    //     }

    //     Destroy(gameObject);
    // }
}