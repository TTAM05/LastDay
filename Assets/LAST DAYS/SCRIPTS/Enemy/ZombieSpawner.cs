using UnityEngine;
using UnityEngine.AI;

public class AmbientZombieSpawner : MonoBehaviour
{
    public GameObject zombiePrefab;

    public Transform[] spawnPoints;

    public float spawnInterval = 2f;

    private bool spawning;

    // =====================================================
    // START
    // =====================================================

    public void StartSpawn()
    {
        spawning = true;

        InvokeRepeating(
            nameof(SpawnZombie),
            0f,
            spawnInterval
        );

        Debug.Log("Ambient Spawner Started");
    }

    // =====================================================
    // STOP
    // =====================================================

    public void StopSpawn()
    {
        spawning = false;

        CancelInvoke();
    }

    // =====================================================
    // SPAWN
    // =====================================================

    void SpawnZombie()
    {
        if (!spawning) return;

        // random point
        Transform point =
            spawnPoints[
                Random.Range(
                    0,
                    spawnPoints.Length
                )
            ];

        Debug.Log("Spawn Point: " + point.position);

        if (NavMesh.SamplePosition(
            point.position,
            out NavMeshHit hit,
            3f,
            NavMesh.AllAreas))
        {
            GameObject zombie = Instantiate(
                zombiePrefab,
                hit.position,
                Quaternion.identity
            );

            // lấy EnemyAI
            EnemyAI ai =
                zombie.GetComponent<EnemyAI>();

            if (ai != null)
            {
                GameObject playerObj =
                    GameObject.FindGameObjectWithTag("Player");

                if (playerObj != null)
                {
                    ai.player = playerObj.transform;

                    ai.SetChaseState();
                }
            }

            Debug.Log("Zombie Spawned");
        }
    }
}