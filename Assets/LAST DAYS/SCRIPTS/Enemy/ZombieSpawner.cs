using UnityEngine;
using UnityEngine.AI;

public class AmbientZombieSpawner : MonoBehaviour
{
    public GameObject zombiePrefab;

    public Transform[] spawnPoints;

    public float spawnInterval = 10f;

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
        if(!spawning) return;

        // random point
        Transform point =
            spawnPoints[
                Random.Range(
                    0,
                    spawnPoints.Length
                )
            ];

        
            if (NavMesh.SamplePosition(
                point.position,
                out NavMeshHit hit,
                3f,
                NavMesh.AllAreas))
            {
                Instantiate(
                    zombiePrefab,
                    hit.position,
                    Quaternion.identity
                );

                Debug.Log("Zombie Spawned");
            }
        

    }
}