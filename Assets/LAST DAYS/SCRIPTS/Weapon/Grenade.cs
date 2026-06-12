using System.Collections;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [Header("Explosion")]
    public float delay = 2f;
    public float radius = 5f;
    public float damage = 200f;
    public float explosionForce = 700f;

    [Header("Effect")]
    public GameObject explosionEffect;

    private bool exploded;

    void Start()
    {
        StartCoroutine(ExplodeAfterDelay());
    }

    IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        Explode();
    }

    void Explode()
    {
        if (exploded) return;
        exploded = true;

        if (explosionEffect != null)
        {
            Instantiate(
                explosionEffect,
                transform.position,
                Quaternion.identity
            );
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider hit in hits)
        {
            EnemyHealth enemy = hit.GetComponentInParent<EnemyHealth>();

            Health player = hit.GetComponentInParent<Health>();

            MutantHealth mutant = hit.GetComponentInParent<MutantHealth>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage, false);
            }

            if (player != null)
            {
                player.TakeDamage(damage);
            }

            if (mutant != null)
            {
                mutant.TakeDamage(damage, false);
            }

            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.AddExplosionForce(
                    explosionForce,
                    transform.position,
                    radius
                );
            }
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            radius
        );
    }

}