using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class LobbyManagerr : MonoBehaviour
{
    public GameObject OptionPanel;
    public TMP_Text MoneyAmount;


    void Start()
    {
        if (OptionPanel != null)
        {
            OptionPanel.SetActive(false);
        }

        LoadMoney();
    }

    void LoadMoney()
    {
        int money = PlayerPrefs.GetInt("Money", 0);

        if (MoneyAmount != null)
        {
            MoneyAmount.text = money.ToString("N0");
        }
    }


    public void OpenOption()
    {
        if (OptionPanel != null)
            OptionPanel.SetActive(true);

    }

    public void CloseOption()
    {
        if (OptionPanel != null)
            OptionPanel.SetActive(false);
    }

    public void LoadChapter()
    {
        SceneManager.LoadScene("MapMenu");
    }

    public void LoadStart()
    {
        SceneManager.LoadScene("Start");
    }

    public void LoadStore()
    {
        SceneManager.LoadScene("Store");
    }
}