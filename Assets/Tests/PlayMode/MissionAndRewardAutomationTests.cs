using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class MissionAndRewardAutomationTests
{
    private GameObject missionObj;
    private Mission mission;
    private GameObject rewardObj;
    private RewardManager rewardManager;
    private AudioClip dummyClip;

    [SetUp]
    public void SetUp()
    {
        PlayerPrefs.DeleteAll();

        dummyClip = AudioClip.Create("DummySound", 44100, 1, 44100, false);

        missionObj = new GameObject("MissionSystem");
        missionObj.SetActive(false);
        mission = missionObj.AddComponent<Mission>();

        GameObject spawnerObj = new GameObject("AmbientSpawner");
        AmbientZombieSpawner spawner = spawnerObj.AddComponent<AmbientZombieSpawner>();
        spawner.spawnPoints = new List<Transform> { spawnerObj.transform };
        mission.ambientSpawner = spawner;

        GameObject markerObj = new GameObject("MarkerSystem");
        MarkerSystem markerSystem = markerObj.AddComponent<MarkerSystem>();
        markerSystem.targets = new Transform[0];
        mission.missionSystem = markerSystem;

        mission.missionUI = new GameObject[] { new GameObject("UI_0"), new GameObject("UI_1") };
        mission.NPC = new GameObject("NPC_Partner");
        mission.NPC.SetActive(false);
        missionObj.SetActive(true);

        rewardObj = new GameObject("RewardManager");
        rewardManager = rewardObj.AddComponent<RewardManager>();
        rewardManager.winPanel = new GameObject("WinPanel");
        rewardManager.starSound = dummyClip;
        rewardManager.stars = new GameObject[]
        {
            new GameObject("Star1"), new GameObject("Star2"), new GameObject("Star3"), new GameObject("Star4"), new GameObject("Star5")
        };
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(missionObj);
        Object.Destroy(rewardObj);
        if (dummyClip != null) Object.Destroy(dummyClip);
        PlayerPrefs.DeleteAll();
    }

    [UnityTest]
    public IEnumerator AutomatedTest_Mission_ExplorePhase_AutoTransitionsToWave1OnTimeout()
    {
        mission.exploreTime = 0.3f;
        mission.StartExplore();

        Assert.AreEqual(GamePhase.Explore, mission.currentPhase, "FAILED: CurrentPhase chưa chuyển sang Explore!");

        yield return new WaitForSeconds(0.4f);

        Assert.AreEqual(GamePhase.Wave1, mission.currentPhase, "FAILED: Hết thời gian Explore nhưng Phase chưa chuyển sang Wave1!");
        Assert.IsTrue(mission.NPC.activeSelf, "FAILED: Chuyển sang Wave1 nhưng NPC không được kích hoạt active!");
    }

    [UnityTest]
    public IEnumerator AutomatedTest_MissionPoint_OnTriggerEnter_StartsExplore()
    {
        GameObject pointObj = new GameObject("MissionPoint_1");
        pointObj.AddComponent<BoxCollider>().isTrigger = true;

        MissionPoint point = pointObj.AddComponent<MissionPoint>();
        point.audioSource = pointObj.AddComponent<AudioSource>();
        point.Clip = dummyClip;

        GameObject player = new GameObject("Player") { tag = "Player" };
        player.AddComponent<BoxCollider>();
        player.AddComponent<Rigidbody>().isKinematic = true;
        player.transform.position = pointObj.transform.position;

        yield return new WaitForFixedUpdate();
        yield return null;

        Assert.AreEqual(GamePhase.Explore, mission.currentPhase, "FAILED: Va chạm MissionPoint không kích hoạt Phase Explore!");

        Object.Destroy(pointObj);
        Object.Destroy(player);
    }

    [UnityTest]
    public IEnumerator AutomatedTest_RewardManager_AntiDoubleClaim_PreventsDuplicateRewards()
    {
        rewardManager.audioSource = rewardObj.AddComponent<AudioSource>();

        rewardManager.GiveWinReward(100f);
        int initialMoney = PlayerPrefs.GetInt("Money", 0);

        Assert.IsTrue(initialMoney > 0, "FAILED: Lần đầu nhận thưởng không được cộng tiền vào PlayerPrefs!");

        rewardManager.GiveWinReward(100f);
        int moneyAfterSecondClaim = PlayerPrefs.GetInt("Money", 0);

        Assert.AreEqual(initialMoney, moneyAfterSecondClaim, "FAILED (BUG THẬT): Hệ thống cho phép nhận thưởng trùng lặp nhiều lần!");
        yield return null;
    }

    [UnityTest]
    public IEnumerator AutomatedTest_RewardManager_FiveStarReward_CalculatesCorrectly()
    {
        rewardManager.audioSource = rewardObj.AddComponent<AudioSource>();

        GameObject npcObj = new GameObject("Partner_NPC");
        NPCHealth npcHealth = npcObj.AddComponent<NPCHealth>();
        CharData dummyChar = ScriptableObject.CreateInstance<CharData>();
        dummyChar.maxHealth = 100f;
        npcHealth.charData = dummyChar;

        yield return null;

        rewardManager.GiveWinReward(150f);

        Assert.AreEqual(5, rewardManager.star, "FAILED: NPC còn sống và nhận < 200 damage phải đạt 5 sao!");
        Assert.AreEqual(5, rewardManager.upgradeShard, "FAILED: Mốc 5 sao phải thưởng đúng 5 UpgradeShard!");

        Object.Destroy(npcObj);
        Object.Destroy(dummyChar);
    }

    [UnityTest]
    public IEnumerator AutomatedTest_RewardManager_MissingAudioSource_CausesNullCrash()
    {
        rewardManager.audioSource = null;

        LogAssert.Expect(LogType.Exception, new Regex(".*NullReferenceException.*"));

        rewardManager.GiveWinReward(100f);
        yield return new WaitForSeconds(1.1f);
    }
}