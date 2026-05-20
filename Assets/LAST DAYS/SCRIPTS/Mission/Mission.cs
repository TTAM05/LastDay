using UnityEngine;
using  UnityEngine.UI;
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

    [Header("UI")]
    public GameObject missionUI;

    // [Header("Wave 1")]
    // public ZombieSpawner[] wave1Spawners;

    void Awake()
    {
        Instance = this;
        missionUI.SetActive(false);
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

        UIMission();

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

    //tắt UI sau 4s
    public void UIMission()
    {
        missionUI.SetActive(true);
        Invoke(nameof(HideMissionUI), 8f);

        Debug.Log("Mission Completed");
    }

    private void HideMissionUI()
    {
        missionUI.SetActive(false);
    }
}