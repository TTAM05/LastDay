using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GamePhase currentPhase;

    [Header("Timer")]
    public float timer;

    [Header("Wave")]
    public int enemiesAlive;

    [Header("Rescue Result")]
    public bool npcSaved;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartExplore();
    }

    void Update()
    {
        timer -= Time.deltaTime;

        switch(currentPhase)
        {
            case GamePhase.Explore:

                if(timer <= 0f)
                {
                    StartWave1();
                }

                break;

            case GamePhase.Wave1:

                if(enemiesAlive <= 0)
                {
                    StartRescueWave();
                }

                break;

            case GamePhase.RescueWave:

                if(timer <= 0f)
                {
                    EndRescueWave();
                }

                break;

            case GamePhase.Wave3:

                if(enemiesAlive <= 0)
                {
                    EndGame();
                }

                break;
        }
    }

    // =====================================================
    // EXPLORE
    // =====================================================

    void StartExplore()
    {
        currentPhase = GamePhase.Explore;

        timer = 90f;

        Debug.Log("Explore Phase");
    }

    // =====================================================
    // WAVE 1
    // =====================================================

    void StartWave1()
    {
        currentPhase = GamePhase.Wave1;

        SpawnWave1();

        Debug.Log("Wave 1");
    }

    // =====================================================
    // RESCUE
    // =====================================================

    void StartRescueWave()
    {
        currentPhase = GamePhase.RescueWave;

        timer = 60f;

        SpawnRescueWave();

        Debug.Log("Rescue Wave");
    }

    void EndRescueWave()
    {
        if(npcSaved)
        {
            Debug.Log("NPC Saved");
        }
        else
        {
            Debug.Log("NPC Dead");
        }

        StartWave3();
    }

    // =====================================================
    // WAVE 3
    // =====================================================

    void StartWave3()
    {
        currentPhase = GamePhase.Wave3;

        SpawnBoss();

        if(npcSaved)
        {
            SpawnSupportNPC();
        }
        else
        {
            SpawnExtraMutants();
        }

        Debug.Log("Boss Wave");
    }

    // =====================================================
    // END
    // =====================================================

    void EndGame()
    {
        currentPhase = GamePhase.End;

        Debug.Log("GAME COMPLETE");
    }

    // =====================================================
    // HELPERS
    // =====================================================

    public void EnemySpawned()
    {
        enemiesAlive++;
    }

    public void EnemyKilled()
    {
        enemiesAlive--;
    }

    // =========================
    // SPAWN METHODS
    // =========================

    void SpawnWave1(){}
    void SpawnRescueWave(){}
    void SpawnBoss(){}
    void SpawnSupportNPC(){}
    void SpawnExtraMutants(){}
}