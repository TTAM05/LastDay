using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class LuckyWheel : MonoBehaviour
{   
    [System.Serializable]
    public class RewardData
    {
        public string rewardName;
        public Sprite icon;
        public int amount;
        public int weight;
        public string popupMessage;

        public Vector2 iconSize = new Vector2(70, 70);
        public Vector2 iconOffset;
        public GameObject rewardPopup;
    }

    [Header("Wheel UI")]
    public RectTransform wheel;
    public Button spinButton;
    public TMP_Text rewardText;

    [Header("Reward")]
    public RewardData[] rewards;
    

    [Header("Spin Setting")]
    public float spinDuration = 4f;
    public int spinRound = 5;
    public float pointerAngle = 0f;
    public float offsetAngle = 0f;

    private bool isSpinning;

    [Header("Popup Close")]
    public GameObject currentPopup;

    [Header("Currency UI")]
    public TMP_Text moneyText;
    public TMP_Text ticketText;
    public TMP_Text shardText;

    void Start()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        moneyText.text =
            PlayerPrefs.GetInt("Money", 0).ToString();

        ticketText.text =
            PlayerPrefs.GetInt("Ticket", 0).ToString();

        shardText.text =
            PlayerPrefs.GetInt("UpgradeShard", 0).ToString();
    }

    public void Spin()
    {
        if (isSpinning) return;

        int rewardIndex = GetRandomRewardIndex();

        StartCoroutine(SpinToReward(rewardIndex));
    }

    int GetRandomRewardIndex()
    {
        int totalWeight = 0;

        for (int i = 0; i < rewards.Length; i++)
        {
            totalWeight += rewards[i].weight;
        }

        int random = Random.Range(0, totalWeight);

        int currentWeight = 0;

        for (int i = 0; i < rewards.Length; i++)
        {
            currentWeight += rewards[i].weight;

            if (random < currentWeight)
            {
                return i;
            }
        }

        return 0;
    }

    IEnumerator SpinToReward(int rewardIndex)
    {
        isSpinning = true;
        spinButton.interactable = false;
        rewardText.text = "";

       float anglePerSlot = 360f / rewards.Length;

        // index ô trúng
        float slotCenterAngle =
            rewardIndex * anglePerSlot;

        // nếu vòng quay lệch nửa ô thì bật dòng này
        // slotCenterAngle += anglePerSlot / 2f;

        float targetRotation =
            pointerAngle - slotCenterAngle + offsetAngle;

        float startZ = wheel.eulerAngles.z;

        float endZ =
            startZ +
            360f * spinRound +
            Mathf.DeltaAngle(startZ, targetRotation);

        float timer = 0f;

        while (timer < spinDuration)
        {
            timer += Time.deltaTime;

            float t = timer / spinDuration;
            t = Mathf.SmoothStep(0f, 1f, t);

            float z = Mathf.Lerp(startZ, endZ, t);

            wheel.eulerAngles =
                new Vector3(0, 0, z);

            yield return null;
        }

        wheel.eulerAngles =
            new Vector3(0, 0, endZ);

        GiveReward(rewardIndex);

        isSpinning = false;
        spinButton.interactable = true;
    }

    void ShowRewardPopup(RewardData reward)
    {
        if (currentPopup != null)
            currentPopup.SetActive(false);

        if (reward.rewardPopup != null)
        {
            currentPopup = reward.rewardPopup;
            currentPopup.SetActive(true);

            TMP_Text text =
                currentPopup.GetComponentInChildren<TMP_Text>();

            if (text != null)
            {
                text.text = reward.popupMessage;
            }
        }
    }

    public void CloseRewardPopup()
    {
        if (currentPopup != null)
        {
            currentPopup.SetActive(false);
            currentPopup = null;
        }
    }

    void GiveReward(int index)
    {
        RewardData reward = rewards[index];

        ShowRewardPopup(reward);

        Debug.Log(
            "Reward: " +
            reward.rewardName +
            " x" +
            reward.amount
        );

        // Ví dụ cộng coin
        if (reward.rewardName == "Money")
        {
            int money = PlayerPrefs.GetInt("Money", 0);
            money += reward.amount;
            PlayerPrefs.SetInt("Money", money);
        }

        // Ví dụ cộng ticket
        if (reward.rewardName == "Ticket")
        {
            int ticket = PlayerPrefs.GetInt("Ticket", 0);
            ticket += reward.amount;
            PlayerPrefs.SetInt("Ticket", ticket);
        }

        //upgradeshard
        if (reward.rewardName == "UpgradeShard")
        {
            int shard = PlayerPrefs.GetInt("UpgradeShard", 0);
            shard += reward.amount;
            PlayerPrefs.SetInt("UpgradeShard", shard);
        }

        //Grenade
        if (reward.rewardName == "Grenade")
        {
            int grenade = PlayerPrefs.GetInt("Grenade", 0);
            grenade += reward.amount;
            PlayerPrefs.SetInt("Grenade", grenade);
        }

        //ProtectCard
        if (reward.rewardName == "ProtectCard")
        {
            int protectCard = PlayerPrefs.GetInt("ProtectCard", 0);
            protectCard += reward.amount;
            PlayerPrefs.SetInt("ProtectCard", protectCard);
        }

        //Kar98
        if (reward.rewardName == "Kar98")
        {
            int kar98 = PlayerPrefs.GetInt("Kar98", 0);
            kar98 += reward.amount;
            PlayerPrefs.SetInt("Kar98", kar98);
        }

        PlayerPrefs.Save();
        UpdateUI();
    }
}