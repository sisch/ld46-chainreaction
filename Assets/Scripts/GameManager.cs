using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Player currentPlayer;
    public int NumberOfPlayers = 1;

    public List<Player> Players;

    public List<Material> Materials;
    public GameObject NeutronPrefab;

    private float timePassed;
    public float NeutronDelay;
    public float timeout;
    public TMP_Text currentPlayerUI;
    public Image currentPlayerPanel;

    public GameObject NeutronGroup;
    private Rod[] grid;
    public int gameWidth;
    public int gameHeight;
    private bool initialized = false;

    public Image WinImage;
    public TMP_Text WinText;

    public TMP_Text[] playerScores;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(this);
        }

        if (Settings.numberOfPlayers == 0)
        {
            Settings.Reset();
        }

        grid = new Rod[gameHeight * gameWidth];
        WinImage.gameObject.SetActive(false);
    }

    private void Start()
    {
        InvokeRepeating("EliminatePlayers", 5f, 5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!initialized)
        {
            Initialize();
        }

        if (Input.GetButtonDown("Cancel"))
        {
            Quit();
        }

        int[] scores = CountRodsPerPlayer();
        UpdateScores(scores);

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F))
        {
            Player p = Players[0];
            currentPlayer = p;
            for (var index = 0; index < grid.Length - 1; index++)
            {
                var rod = grid[index];
                p.SpawnNewNeutron();
                rod.AddNeutron(p.newNeutron);
                p.newNeutron.SetTarget(rod.target);
                rod.SetOwner(p);
            }

            NextPlayer();
        }
#endif
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.G))
        {
            Player p = Players[1];
            currentPlayer = p;
            for (var index = 0; index < grid.Length - 1; index++)
            {
                var rod = grid[index];
                p.SpawnNewNeutron();
                rod.AddNeutron(p.newNeutron);
                p.newNeutron.SetTarget(rod.target);
                rod.SetOwner(p);
            }

            NextPlayer();
        }
#endif
    }

    private void UpdateScores(int[] scores)
    {
        for (int i = 0; i < scores.Length; i++)
        {
            playerScores[i].SetText($"{Players[i].name}: {scores[i]:00}");
        }
    }

    private int[] CountRodsPerPlayer()
    {
        int[] scores = new int[NumberOfPlayers];
        for (int i = 0; i < scores.Length; i++)
        {
            scores[i] = 0;
        }

        foreach (var rod in grid)
        {
            var index = Players.IndexOf(rod.ownedBy);
            if (index >= 0)
            {
                scores[index]++;
            }
        }

        return scores;
    }

    void EliminatePlayers()
    {
        foreach (var player in Players)
        {
            if (player == null || !HasValidMoves(player))
            {
                Players.Remove(player);
            }
        }

        if (Players.Count == 0)
        {
            ShowWinScreen(null);
        }
        else if (Players.Count == 1)
        {
            ShowWinScreen(Players[0]);
        }
    }

    void Initialize()
    {
        NumberOfPlayers = Settings.numberOfPlayers;
        if (Players == null)
        {
            Players = new List<Player>(NumberOfPlayers);
        }

        for (int i = 0; i < NumberOfPlayers; i++)
        {
            if (i < Players.Count)
            {
                continue;
            }

            var player = gameObject.AddComponent<Player>();
            player.name = $"Player {i + 1}";
            player.playerMat = Utility.PopRandom(ref Materials);
            player.NeutronGroupTransform = NeutronGroup.transform;
            Players.Add(player);
        }

        for (var index = 0; index < playerScores.Length; index++)
        {
            var playerScore = playerScores[index];
            var activeIndex = index < NumberOfPlayers;
            playerScore.gameObject.SetActive(activeIndex);
            if (activeIndex)
            {
                playerScore.color = Players[index].playerMat.GetColor("_BaseColor");
            }
        }

        NextPlayer();
        initialized = true;
    }


    public void InvokeNextPlayer()
    {
        StartCoroutine(NextPlayerTimer());
    }

    public void ResetTimer()
    {
        timePassed = 0;
    }

    IEnumerator NextPlayerTimer()
    {
        if (Settings.useDelay)
        {
            while (timePassed <= timeout)
            {
                timePassed += Time.deltaTime;
                yield return null;
            }
        }

        NextPlayer();
        timePassed = 0;
    }

    void NextPlayer()
    {
        if (currentPlayer != null)
        {
            currentPlayer.isActive = false;
        }

        if (Players.Count == 1)
        {
            currentPlayerUI.SetText($"{Players[0].name} wins!");
            ShowWinScreen(Players[0]);
        }

        var index = (Players.IndexOf(currentPlayer) + 1) % Players.Count;
        if (!HasValidMoves(Players[index]))
        {
            Players.Remove(Players[index]);
            NextPlayer();
            return;
        }

        currentPlayer = Players[index];
        currentPlayer.Activate();
        currentPlayerUI.SetText(currentPlayer.name);
        currentPlayerPanel.color = currentPlayer.playerMat.GetColor("_BaseColor");
    }

    private bool HasValidMoves(Player player)
    {
        bool returnval = false;
        foreach (Rod rod in grid)
        {
            if (rod.ownedBy == player || rod.ownedBy == null)
            {
                return true;
            }
        }

        return false;
    }

    private void ShowWinScreen(Player player)
    {
        WinText.SetText($"{player.name} wins");
        WinImage.color = player.playerMat.GetColor("_BaseColor");
        WinImage.gameObject.SetActive(true);
    }

    public void RegisterRod(Rod rod)
    {
        grid[rod.gridCoordinate.x + gameWidth * rod.gridCoordinate.y] = rod;
    }

    public Rod GetRodAt(Rod.GridCoordinate coord)
    {
        if (coord.x >= gameWidth || coord.x < 0 || coord.y >= gameHeight || coord.y < 0)
        {
            return null;
        }

        return grid[coord.x + gameWidth * coord.y];
    }

    public void Quit()
    {
        SceneManager.LoadScene("menu");
    }
}