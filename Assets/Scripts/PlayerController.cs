using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed     = 4f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float gravity       = -9.81f;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private CharacterController _cc;
    private Vector3 _velocity;
    private bool _canMove = true;

    private static readonly int SpeedHash = 
        Animator.StringToHash("Speed");

    private void Awake()
    {
        // Get CharacterController on same object
        _cc = GetComponent<CharacterController>();

        if (_cc == null)
            Debug.LogError("[PlayerController] No CharacterController found!");

        // Auto find animator in children
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        // Check if we can move
        if (!_canMove)
        {
            if (animator != null)
                animator.SetFloat(SpeedHash, 0f);
            return;
        }

        // Check GameManager exists
        if (GameManager.Instance != null && 
            GameManager.Instance.IsGameOver)
            return;

        HandleMovement();
        HandleGravity();
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical   = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(
            horizontal, 0f, vertical
        ).normalized;

        float speed = direction.magnitude;

        // Update walk animation
        if (animator != null)
            animator.SetFloat(SpeedHash, speed);

        if (speed >= 0.1f)
        {
            // Calculate rotation angle
            float targetAngle = Mathf.Atan2(
                direction.x, direction.z
            ) * Mathf.Rad2Deg;

            // Smooth rotation
            float smoothAngle = Mathf.LerpAngle(
                transform.eulerAngles.y,
                targetAngle,
                rotationSpeed * Time.deltaTime
            );

            transform.rotation = Quaternion.Euler(
                0f, smoothAngle, 0f
            );

            // Move the player
            _cc.Move(direction * moveSpeed * Time.deltaTime);

            Debug.Log($"[Player] Moving: {direction} speed: {speed}");
        }
    }

    private void HandleGravity()
    {
        if (_cc.isGrounded && _velocity.y < 0f)
            _velocity.y = -2f;

        _velocity.y += gravity * Time.deltaTime;
        _cc.Move(_velocity * Time.deltaTime);
    }

    public void SetMovementEnabled(bool enabled)
    {
        _canMove = enabled;
        if (!enabled && animator != null)
            animator.SetFloat(SpeedHash, 0f);
    }

    public void Teleport(Vector3 worldPosition)
    {
        _cc.enabled = false;
        transform.position = worldPosition;
        _cc.enabled = true;
        Debug.Log($"[Player] Teleported to {worldPosition}");
    }
}