using UnityEngine;
using System.Collections;

public class LessonManager : MonoBehaviour
{
    [System.Serializable]
    public class Lesson
    {
        [TextArea(2, 4)] public string instruction;
        public float displayDuration     = 4f;
        public bool  waitForPlayerAction = false;
    }

    [Header("Lesson Content")]
    [SerializeField] private Lesson[] lessons;

    [Header("UI")]
    [SerializeField] private TutorialPanel tutorialPanel;

    [Header("Colours")]
    [SerializeField] private Color hintColour    = Color.white;
    [SerializeField] private Color warningColour = Color.yellow;
    [SerializeField] private Color successColour = Color.green;

    private int      _currentLessonIndex = 0;
    private bool     _waitingForPlayer   = false;
    private Coroutine _autoAdvance;

    private void Start()
    {
        if (lessons.Length > 0)
            ShowLesson(0);
    }

    private void ShowLesson(int index)
    {
        if (index >= lessons.Length) return;
        Lesson lesson = lessons[index];
        tutorialPanel?.ShowMessage(lesson.instruction, hintColour);

        if (lesson.waitForPlayerAction)
            _waitingForPlayer = true;
        else if (lesson.displayDuration > 0f)
        {
            if (_autoAdvance != null) StopCoroutine(_autoAdvance);
            _autoAdvance = StartCoroutine(AutoAdvance(lesson.displayDuration));
        }
    }

    private IEnumerator AutoAdvance(float delay)
    {
        yield return new WaitForSeconds(delay);
        AdvanceLesson();
    }

    public void AdvanceLesson()
    {
        _waitingForPlayer = false;
        _currentLessonIndex++;
        if (_currentLessonIndex < lessons.Length)
            ShowLesson(_currentLessonIndex);
        else
            tutorialPanel?.HidePanel();
    }

    public void ShowHint(string message)    => tutorialPanel?.ShowMessage(message, hintColour, 3f);
    public void ShowWarning(string message) => tutorialPanel?.ShowMessage(message, warningColour, 4f);
    public void ShowSuccess(string message) => tutorialPanel?.ShowMessage(message, successColour, 3f);

    public void NotifyPlayerAction()
    {
        if (_waitingForPlayer) AdvanceLesson();
    }
}