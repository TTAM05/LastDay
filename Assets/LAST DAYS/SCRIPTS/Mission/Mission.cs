using UnityEngine;
using  UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    //Mission 0
    [Header("Phase")]
    public GamePhase currentPhase;

    [Header("Explore")]
    public float exploreTime = 60f;

    private float timer;

    [Header("Ambient")]
    public AmbientZombieSpawner ambientSpawner;

    [Header("Mission System")]
    public MissionSystem missionSystem;

    [Header("UI")]
    public GameObject[] missionUI;

    //Mission 1
    [Header("Wave 1")]
    public GameObject NPC;
    
    void Awake()
    {
        Instance = this;
        currentPhase = GamePhase.None;

        foreach (var ui in missionUI)
            ui.SetActive(false);

        if (NPC != null)
            NPC.SetActive(false);
    }

    void Update()
    {
        if(currentPhase == GamePhase.Explore)
        {
            timer -= Time.deltaTime;

            if(timer <= 0f)
            {
                StartWave1();
                if (missionSystem != null)
                    missionSystem.CompleteCurrentMission();
                else
                    Debug.LogWarning("MissionSystem reference is missing on GameManager.");
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

        UIMission(0);

        Debug.Log("Explore Started");

       
    }

    // =====================================================
    // WAVE 1
    // =====================================================
    public void StartWave1()
    {
        currentPhase = GamePhase.Wave1;

        ambientSpawner.StopSpawn();

        if (NPC != null)
            NPC.SetActive(true);

        UIMission(1);

        Debug.Log("Wave 1 Started");
    }

    //tắt UI sau 4s
    public void UIMission(int missionIndex)
    {
        HideMissionUI();

        if (missionIndex < 0 || missionIndex >= missionUI.Length)
            return;

        missionUI[missionIndex].SetActive(true);

        CancelInvoke(nameof(HideMissionUI));
        Invoke(nameof(HideMissionUI), 8f);
    }

    private void HideMissionUI()
    {
        foreach (var ui in missionUI)
        {
            ui.SetActive(false);
        }
    }
}