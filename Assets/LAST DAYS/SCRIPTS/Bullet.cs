using UnityEngine;

public class Bullet : MonoBehaviour
{
    // thời gian tự hủy
    public float lifeTime = 3f;

    void Start()
    {
        // tự hủy
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hit: " + other.name);

        Destroy(gameObject);
    }
}