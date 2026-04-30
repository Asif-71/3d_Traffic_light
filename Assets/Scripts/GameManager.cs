using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Events for HUD and UI
    public event Action<int> OnScoreChanged;
    public event Action<int> OnLivesChanged;
    public event Action OnGameOver;
    public event Action OnLevelComplete;

    // Gameplay values
    public int Score { get; private set; }
    public int Lives { get; private set; }

    [Header("Initial Values")]
    [SerializeField] private int startingLives = 3;

    [Header("Respawn Settings")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform spawnPoint;
    private GameObject currentPlayer;

    // State flags
    public bool IsGameOver { get; private set; }
    public bool IsPaused { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Score = 0;
        Lives = startingLives;
        IsGameOver = false;
        IsPaused = false;

        RespawnPlayer();
    }

    // 🔹 Add points
    public void AddScore(int amount)
    {
        Score += amount;
        Debug.Log("Score increased to: " + Score);
        OnScoreChanged?.Invoke(Score);
    }

    // 🔹 Safe crossing check (bool version)
    public void RegisterSafeCrossing(bool isSafe)
    {
        if (isSafe)
        {
            AddScore(10); // award points
            Debug.Log("Safe crossing registered!");
        }
        else
        {
            LoseLife(); // unsafe crossing costs a life
            Debug.Log("Unsafe crossing!");
        }
    }

    // 🔹 SafeZone success (direct call from SafeZone.cs)
    public void RegisterSafeZoneSuccess()
    {
        AddScore(10); // award points
        Debug.Log("Player reached SafeZone! Score awarded.");
    }

    // 🔹 Player hit by car
    public void RegisterPlayerHit()
    {
        Debug.Log("Player was hit!");
        LoseLife();
    }

    // 🔹 Lose a life
    public void LoseLife()
    {
        Lives--;
        OnLivesChanged?.Invoke(Lives);

        if (Lives > 0)
        {
            RespawnPlayer();
        }
        else
        {
            HandleGameOver();
        }
    }

    // 🔹 Respawn player capsule
    public void RespawnPlayer()
    {
        if (currentPlayer != null) Destroy(currentPlayer);
        currentPlayer = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
    }

    // 🔹 Reset game
    public void ResetGame()
    {
        Score = 0;
        Lives = startingLives;
        IsGameOver = false;
        IsPaused = false;

        OnScoreChanged?.Invoke(Score);
        OnLivesChanged?.Invoke(Lives);

        RespawnPlayer();
    }

    // 🔹 Pause / Resume
    public void PauseGame()
    {
        IsPaused = true;
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        IsPaused = false;
        Time.timeScale = 1f;
    }

    // 🔹 Complete level
    public void CompleteLevel()
    {
        Debug.Log("Level Complete!");
        OnLevelComplete?.Invoke();
    }

    // 🔹 Handle game over
    private void HandleGameOver()
    {
        IsGameOver = true;
        Debug.Log("Game Over! No lives left.");
        OnGameOver?.Invoke();
    }
}
