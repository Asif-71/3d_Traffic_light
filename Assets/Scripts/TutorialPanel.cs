using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TutorialPanel : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text    messageText;

    [Header("Animation")]
    [SerializeField] private float fadeInDuration  = 0.3f;
    [SerializeField] private float fadeOutDuration = 0.5f;

    private Coroutine _currentCoroutine;

    private void Awake()
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }

    public void ShowMessage(string message, Color textColor, float duration = 0f)
    {
        if (_currentCoroutine != null) StopCoroutine(_currentCoroutine);
        messageText.text  = message;
        messageText.color = textColor;
        _currentCoroutine = StartCoroutine(DisplayRoutine(duration));
    }

    public void HidePanel()
    {
        if (_currentCoroutine != null) StopCoroutine(_currentCoroutine);
        _currentCoroutine = StartCoroutine(FadeOut());
    }

    private IEnumerator DisplayRoutine(float duration)
    {
        gameObject.SetActive(true);
        yield return StartCoroutine(FadeIn());

        if (duration > 0f)
        {
            yield return new WaitForSeconds(duration);
            yield return StartCoroutine(FadeOut());
        }
    }

    private IEnumerator FadeIn()
    {
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    private IEnumerator FadeOut()
    {
        float elapsed    = 0f;
        float startAlpha = canvasGroup.alpha;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / fadeOutDuration);
            yield return null;
        }
        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }
}