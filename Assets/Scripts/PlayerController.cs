using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float gravity = -9.81f;

    private CharacterController _cc;
    private Vector3 _velocity;
    private bool _canMove = true;

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (!_canMove || GameManager.Instance == null || GameManager.Instance.IsGameOver)
            return;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical   = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float smoothAngle = Mathf.LerpAngle(transform.eulerAngles.y, targetAngle,
                                                 rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);
            _cc.Move(direction * moveSpeed * Time.deltaTime);
        }

        if (_cc.isGrounded && _velocity.y < 0f)
            _velocity.y = -2f;

        _velocity.y += gravity * Time.deltaTime;
        _cc.Move(_velocity * Time.deltaTime);
    }

    public void SetMovementEnabled(bool enabled)
    {
        _canMove = enabled;
    }

    public void Teleport(Vector3 worldPosition)
    {
        // Debug message to confirm teleport is called
        Debug.Log("Teleporting player to: " + worldPosition);

        // Temporarily disable CharacterController to avoid conflicts
        _cc.enabled = false;

        // Move player to the respawn point
        transform.position = worldPosition;

        // Re-enable CharacterController
        _cc.enabled = true;
    }
}
