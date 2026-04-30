using UnityEngine;

public class CrossingEvaluator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TrafficLight nearestLight;
    [SerializeField] private LessonManager lessonManager;

    [Header("Evaluation")]
    [SerializeField] private float lookPauseDuration = 1.5f;

    private bool  _playerInWaitZone          = false;
    private bool  _playerLookedBeforeCrossing = false;
    private bool  _crossedOnGreen            = false;
    private bool  _crossingStarted           = false;
    private float _pauseTimer                = 0f;

    private void Update()
    {
        if (!_playerInWaitZone || _crossingStarted) return;

        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if (input.magnitude < 0.05f)
        {
            _pauseTimer += Time.deltaTime;
            if (_pauseTimer >= lookPauseDuration && !_playerLookedBeforeCrossing)
            {
                _playerLookedBeforeCrossing = true;
                lessonManager?.ShowHint("Good! You stopped and looked. Now wait for green.");
                //AudioManager.Instance?.PlaySFX("approval");
            }
        }
        else
        {
            _pauseTimer = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (!_playerInWaitZone)
        {
            _playerInWaitZone = true;
            lessonManager?.ShowHint("Stop here. Look both ways before you cross!");
        }
        else
        {
            EvaluateCrossing();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (_playerInWaitZone && !_crossingStarted)
        {
            _crossingStarted = true;
            _crossedOnGreen  = nearestLight != null && nearestLight.IsSafeForPedestrian();

            if (!_playerLookedBeforeCrossing)
                lessonManager?.ShowWarning("You didn't stop to look! Always look before crossing.");

            if (!_crossedOnGreen)
                lessonManager?.ShowWarning("The light was RED! Wait for green before crossing.");
        }
    }

    private void EvaluateCrossing()
    {
        bool success = _playerLookedBeforeCrossing && _crossedOnGreen;

        if (success)
        {
            GameManager.Instance?.RegisterSafeCrossing(_crossedOnGreen);
            lessonManager?.ShowSuccess("Well done! You crossed safely.");
            AudioManager.Instance?.PlaySFX("success");
        }
        else
        {
            lessonManager?.ShowWarning("You were lucky this time — always follow the rules!");
        }

        ResetEvaluation();
    }

    private void ResetEvaluation()
    {
        _playerInWaitZone           = false;
        _playerLookedBeforeCrossing = false;
        _crossedOnGreen             = false;
        _crossingStarted            = false;
        _pauseTimer                 = 0f;
    }
}