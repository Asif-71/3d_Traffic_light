using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Invincibility")]
    [SerializeField] private float invincibilityDuration = 2f;
    [SerializeField] private float flashInterval = 0.15f;

    [Header("Respawn")]
    [SerializeField] private Transform respawnPoint;

    [Header("References")]
    [SerializeField] private Renderer[] playerRenderers;
    [SerializeField] private PlayerController playerController;

    private bool _isInvincible = false;

    private void Awake()
    {
        if (playerController == null)
            playerController = GetComponent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isInvincible) return;

        if (other.CompareTag("Vehicle"))
            StartCoroutine(HitSequence());
    }

   private IEnumerator HitSequence()
{
    if (GameManager.Instance == null || GameManager.Instance.IsGameOver) yield break;

    _isInvincible = true;
    playerController.SetMovementEnabled(false);
    GameManager.Instance.RegisterPlayerHit();

    yield return new WaitForSecondsRealtime(0.5f);

    // Respawn player
    Vector3 spawnPos = respawnPoint != null 
        ? respawnPoint.position 
        : new Vector3(0, 1, -15);
    
    playerController.Teleport(spawnPos);
    playerController.SetMovementEnabled(true);

    // Flash the player
    float elapsed = 0f;
    bool visible = true;

    while (elapsed < invincibilityDuration)
    {
        visible = !visible;
        foreach (var r in playerRenderers)
        {
            if (r != null) r.enabled = visible;
        }
        yield return new WaitForSeconds(flashInterval);
        elapsed += flashInterval;
    }

    // Make sure player is visible at end
    foreach (var r in playerRenderers)
    {
        if (r != null) r.enabled = true;
    }

    _isInvincible = false;
}
}