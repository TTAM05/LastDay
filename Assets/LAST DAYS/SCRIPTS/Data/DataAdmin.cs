using UnityEngine;
using TMPro;

public class DebugDataController : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text dataText;
    public TMP_InputField valueInput;

    const string MONEY_KEY = "Money";
    const string UPGRADE_SHARD_KEY = "UpgradeShard";
    const string TICKET_KEY = "Ticket";

    void Start()
    {
        RefreshUI();
    }

    public void AddMoney() => AddValue(MONEY_KEY);
    public void SubtractMoney() => SubtractValue(MONEY_KEY);
    public void ResetMoney() => ResetValue(MONEY_KEY);

    public void AddUpgradeShard() => AddValue(UPGRADE_SHARD_KEY);
    public void SubtractUpgradeShard() => SubtractValue(UPGRADE_SHARD_KEY);
    public void ResetUpgradeShard() => ResetValue(UPGRADE_SHARD_KEY);

    public void AddTicket() => AddValue(TICKET_KEY);
    public void SubtractTicket() => SubtractValue(TICKET_KEY);
    public void ResetTicket() => ResetValue(TICKET_KEY);

    public void ResetAllData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        RefreshUI();
    }

    void AddValue(string key)
    {
        int value = PlayerPrefs.GetInt(key, 0);
        value += GetInputValue();

        PlayerPrefs.SetInt(key, value);
        PlayerPrefs.Save();

        RefreshUI();
    }

    void SubtractValue(string key)
    {
        int value = PlayerPrefs.GetInt(key, 0);
        value -= GetInputValue();
        value = Mathf.Max(0, value);

        PlayerPrefs.SetInt(key, value);
        PlayerPrefs.Save();

        RefreshUI();
    }

    void ResetValue(string key)
    {
        PlayerPrefs.SetInt(key, 0);
        PlayerPrefs.Save();

        RefreshUI();
    }

    int GetInputValue()
    {
        if (valueInput == null)
            return 0;

        int.TryParse(valueInput.text, out int value);
        return Mathf.Max(0, value);
    }

    void RefreshUI()
    {
        int money = PlayerPrefs.GetInt(MONEY_KEY, 0);
        int shard = PlayerPrefs.GetInt(UPGRADE_SHARD_KEY, 0);
        int ticket = PlayerPrefs.GetInt(TICKET_KEY, 0);

        if (dataText != null)
        {
            dataText.text =
                $"Money: {money:N0}\n" +
                $"Upgrade Shard: {shard:N0}\n" +
                $"Ticket: {ticket:N0}";
        }
    }
}