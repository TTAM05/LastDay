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

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hit: " + other.name);

        // if (other.CompareTag("Enemy"))
        // {
        //     EnemyHealth enemy =
        //         other.GetComponent<EnemyHealth>();

        //     if (enemy != null)
        //     {
        //         enemy.TakeDamage(damage);
        //     }

        //     Destroy(gameObject);
        // }

        Destroy(gameObject);
    }
}