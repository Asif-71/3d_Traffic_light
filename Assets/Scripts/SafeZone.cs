using UnityEngine;

public class SafeZone : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool isGoalZone = true;
    [SerializeField] private bool triggerLevelComplete = false;

    [Header("References")]
    [SerializeField] private LessonManager lessonManager;

    [Header("Visual Feedback")]
    [SerializeField] private GameObject successParticles;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (isGoalZone)
        {
            if (successParticles != null)
                Instantiate(successParticles, transform.position, Quaternion.identity);

            lessonManager?.NotifyPlayerAction();

            // ✅ Guaranteed scoring call
            GameManager.Instance?.RegisterSafeZoneSuccess();

            if (triggerLevelComplete)
                GameManager.Instance?.CompleteLevel();
        }
    }
}
