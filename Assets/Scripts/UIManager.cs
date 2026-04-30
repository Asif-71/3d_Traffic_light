using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject levelCompletePanel;
    [SerializeField] private GameObject hudPanel;

    [Header("Game Over")]
    [SerializeField] private TMP_Text gameOverScoreText;
    [SerializeField] private Button   gameOverRestartButton;
    [SerializeField] private Button   gameOverMenuButton;

    [Header("Level Complete")]
    [SerializeField] private TMP_Text levelCompleteScoreText;
    [SerializeField] private Button   nextLevelButton;

    [Header("Pause")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button pauseMenuButton;

    private void Start()
    {
        // Hide all panels except HUD at start
        SetActive(pausePanel,         false);
        SetActive(gameOverPanel,      false);
        SetActive(levelCompletePanel, false);
        SetActive(hudPanel,           true);

        // Subscribe to GameManager events
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameOver      += ShowGameOver;
            GameManager.Instance.OnLevelComplete += ShowLevelComplete;
        }

        // Wire up all buttons
        resumeButton?.onClick.AddListener(OnResume);
        pauseMenuButton?.onClick.AddListener(OnMainMenu);
        gameOverRestartButton?.onClick.AddListener(OnRestart);
        gameOverMenuButton?.onClick.AddListener(OnMainMenu);
        nextLevelButton?.onClick.AddListener(OnNextLevel);
    }

    private void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameOver      -= ShowGameOver;
            GameManager.Instance.OnLevelComplete -= ShowLevelComplete;
        }
    }

    private void Update()
    {
        // Press Escape to pause/unpause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameManager.Instance != null && !GameManager.Instance.IsGameOver)
                TogglePause();
        }
    }

    private void TogglePause()
    {
        bool paused = GameManager.Instance.IsPaused;
        if (paused)
        {
            GameManager.Instance.ResumeGame();
            SetActive(pausePanel, false);
        }
        else
        {
            GameManager.Instance.PauseGame();
            SetActive(pausePanel, true);
        }
    }

    private void ShowGameOver()
    {
        SetActive(hudPanel,      false);
        SetActive(gameOverPanel, true);

        if (gameOverScoreText != null && GameManager.Instance != null)
            gameOverScoreText.text = $"Score: {GameManager.Instance.Score}";
    }

    private void ShowLevelComplete()
    {
        SetActive(levelCompletePanel, true);

        if (levelCompleteScoreText != null && GameManager.Instance != null)
            levelCompleteScoreText.text = $"Score: {GameManager.Instance.Score}";
    }

    private void OnResume()
    {
        GameManager.Instance?.ResumeGame();
        SetActive(pausePanel, false);
    }

    private void OnRestart()
    {
        GameManager.Instance?.ResetGame();
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnNextLevel()
    {
        // Reload current scene for now
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnMainMenu()
    {
        GameManager.Instance?.ResetGame();
        // Reload current scene for now
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private static void SetActive(GameObject panel, bool active)
    {
        if (panel != null) panel.SetActive(active);
    }
}