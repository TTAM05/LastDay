using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Phase")]
    public GamePhase currentPhase;

    [Header("Explore")]
    public float exploreTime = 90f;

    private float timer;

    [Header("Ambient")]
    public AmbientZombieSpawner ambientSpawner;

    // [Header("Wave 1")]
    // public ZombieSpawner[] wave1Spawners;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if(currentPhase == GamePhase.Explore)
        {
            timer -= Time.deltaTime;

            if(timer <= 0f)
            {
                StartWave1();
            }
        }
    }

    // =====================================================
    // PLAYER CHẠM POINT 1
    // =====================================================

    public void StartExplore()
    {
        currentPhase = GamePhase.Explore;

        timer = exploreTime;

        ambientSpawner.StartSpawn();

        Debug.Log("Explore Started");
    }

    // =====================================================
    // WAVE 1
    // =====================================================

    void StartWave1()
    {
        currentPhase = GamePhase.Wave1;

        // ambientSpawner.StopSpawn();

        // foreach(var spawner in wave1Spawners)
        // {
        //     spawner.Spawn();
        // }

        Debug.Log("Wave 1 Started");
    }
}