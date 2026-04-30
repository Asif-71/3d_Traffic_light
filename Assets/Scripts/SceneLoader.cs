using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    [SerializeField] private CanvasGroup fadeOverlay;
    [SerializeField] private float fadeDuration = 0.5f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(FadeAndLoad(sceneName));
    }

    private IEnumerator FadeAndLoad(string sceneName)
    {
        // Fade to black only if overlay is assigned
        if (fadeOverlay != null)
            yield return StartCoroutine(Fade(0f, 1f));

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);

        // Stop if scene is not found in Build Settings
        if (op == null)
        {
            Debug.LogError($"[SceneLoader] Scene '{sceneName}' not found! " +
                           $"Go to File > Build Profiles and add it.");
            yield break;
        }

        while (!op.isDone)
            yield return null;

        // Fade back in only if overlay is assigned
        if (fadeOverlay != null)
            yield return StartCoroutine(Fade(1f, 0f));
    }

    private IEnumerator Fade(float from, float to)
    {
        if (fadeOverlay == null) yield break;

        float elapsed = 0f;
        fadeOverlay.gameObject.SetActive(true);
        fadeOverlay.alpha = from;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            fadeOverlay.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            yield return null;
        }

        fadeOverlay.alpha = to;

        if (to == 0f)
            fadeOverlay.gameObject.SetActive(false);
    }
}