using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public TMP_Text numberPlayersUI;
    public Toggle delayToggleUI;
    public void Quit()
    {
        Application.Quit();
    }

    public void Awake()
    {
        Settings.Reset();
        UpdateSettings();
    }

    public void NewGame()
    {
        SceneManager.LoadScene("game");
    }

    public void ChangePlayers(int value)
    {
        int players = Settings.numberOfPlayers + value;
        players = Mathf.Max(2, players);
        players = Mathf.Min(8, players);
        Settings.numberOfPlayers = players;
        UpdateSettings();
    }

    public void ToggleDelay()
    {
        Settings.useDelay = !Settings.useDelay;
        UpdateSettings();
    }

    public void UpdateSettings()
    {
        numberPlayersUI.SetText($"{Settings.numberOfPlayers} players");
        delayToggleUI.isOn = Settings.useDelay;
    }
}
