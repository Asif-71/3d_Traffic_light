using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDController : MonoBehaviour
{
    [Header("Score and Lives")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text livesText;

    [Header("Traffic Light Status")]
    [SerializeField] private Image    lightStatusImage;
    [SerializeField] private TMP_Text lightStatusText;
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private Color    safeColor   = Color.green;
    [SerializeField] private Color    dangerColor = Color.red;
    [SerializeField] private Color    waitColor   = Color.yellow;

    [Header("Reference")]
    [SerializeField] private TrafficLight monitoredLight;

    private void OnEnable()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.OnScoreChanged += UpdateScore;
        GameManager.Instance.OnLivesChanged += UpdateLives;
    }

    private void OnDisable()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.OnScoreChanged -= UpdateScore;
        GameManager.Instance.OnLivesChanged -= UpdateLives;
    }

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            UpdateScore(GameManager.Instance.Score);
            UpdateLives(GameManager.Instance.Lives);
        }
    }

    private void Update()
    {
        if (monitoredLight == null)
        {
            Debug.Log("NO LIGHT CONNECTED");
            return;
        }

        // ✅ Get remaining time
        float time = monitoredLight.GetRemainingTime();
        Debug.Log($"Timer: {time}");

        int timeLeft = Mathf.CeilToInt(time);

        // ✅ Update countdown text
        if (countdownText != null)
        {
            countdownText.text = $"Countdown: {timeLeft}s";
        }

        // ✅ Update pedestrian-safe light status and colors
        if (lightStatusImage != null)
        {
            switch (monitoredLight.CurrentState)
            {
                case TrafficLight.LightState.Green:
                    // Cars moving → pedestrians stop
                    lightStatusImage.color = dangerColor;
                    if (lightStatusText != null) lightStatusText.text = "DO NOT CROSS";
                    if (countdownText != null) countdownText.color = dangerColor;
                    break;

                case TrafficLight.LightState.Amber:
                    // Transition → pedestrians wait
                    lightStatusImage.color = waitColor;
                    if (lightStatusText != null) lightStatusText.text = "WAIT";
                    if (countdownText != null) countdownText.color = waitColor;
                    break;

                case TrafficLight.LightState.Red:
                    // Cars stopped → pedestrians cross
                    lightStatusImage.color = safeColor;
                    if (lightStatusText != null) lightStatusText.text = "CROSS NOW";
                    if (countdownText != null) countdownText.color = safeColor;
                    break;
            }
        }
    }

    private void UpdateScore(int score)
    {
        if (scoreText != null) scoreText.text = $"Score: {score}";
    }

    private void UpdateLives(int lives)
    {
        if (livesText != null) livesText.text = $"Lives: {lives}";
    }
}
