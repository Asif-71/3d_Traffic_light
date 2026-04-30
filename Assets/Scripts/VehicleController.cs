using UnityEngine;

public class VehicleController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private Vector3 driveDirection = Vector3.forward;
    [SerializeField] private float despawnDistance  = 60f;

    [Header("Stop Line")]
    [SerializeField] private float stopLineZ      = 0f;
    [SerializeField] private float brakingDistance = 5f;

    [Header("Traffic Light")]
    [SerializeField] private TrafficLight assignedLight;

    private Vector3 _spawnPosition;
    private bool    _stopped = false;
    private float   _currentSpeed;

    private void Start()
    {
        _spawnPosition = transform.position;
        _currentSpeed  = speed;

        if (assignedLight != null)
            assignedLight.OnStateChanged += OnLightChanged;
    }

    private void OnDestroy()
    {
        if (assignedLight != null)
            assignedLight.OnStateChanged -= OnLightChanged;
    }

    private void Update()
    {
        HandleBraking();

        if (!_stopped)
            transform.Translate(driveDirection * _currentSpeed * Time.deltaTime, Space.World);

        if (Vector3.Distance(_spawnPosition, transform.position) > despawnDistance)
            TrafficManager.Instance?.RecycleVehicle(this);
    }

    private void HandleBraking()
    {
        if (assignedLight == null) return;

        bool isRed = assignedLight.CurrentState == TrafficLight.LightState.Red
                  || assignedLight.CurrentState == TrafficLight.LightState.Amber;

        float dist = Mathf.Abs(transform.position.z - stopLineZ);

        if (isRed && dist < brakingDistance && !_stopped)
        {
            _currentSpeed = speed * (dist / brakingDistance);
            if (dist < 0.5f) { _stopped = true; _currentSpeed = 0f; }
        }
    }

    private void OnLightChanged(TrafficLight.LightState newState)
    {
        if (newState == TrafficLight.LightState.Green)
        {
            _stopped = false;
            _currentSpeed = speed;
        }
    }

    public void ResetToSpawn(Vector3 pos, float newSpeed, TrafficLight light)
    {
        transform.position = pos;
        _spawnPosition     = pos;
        speed              = newSpeed;
        _currentSpeed      = newSpeed;
        _stopped           = false;
        AssignTrafficLight(light);
    }

    public void AssignTrafficLight(TrafficLight light)
    {
        if (assignedLight != null)
            assignedLight.OnStateChanged -= OnLightChanged;

        assignedLight = light;

        if (assignedLight != null)
            assignedLight.OnStateChanged += OnLightChanged;
    }
}