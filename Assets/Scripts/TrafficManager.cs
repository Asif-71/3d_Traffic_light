using UnityEngine;
using System.Collections.Generic;

public class TrafficManager : MonoBehaviour
{
    public static TrafficManager Instance { get; private set; }

    [System.Serializable]
    public class Lane
    {
        public string        laneName;
        public Transform     spawnPoint;
        public Vector3       driveDirection;
        public TrafficLight  trafficLight;
        public float         stopLineZ;
        public float         minSpeed = 6f;
        public float         maxSpeed = 12f;
        public float         spawnInterval = 3f;
    }

    [Header("Lanes")]
    [SerializeField] private Lane[] lanes;

    [Header("Vehicles")]
    [SerializeField] private GameObject[] vehiclePrefabs;
    [SerializeField] private int poolSizePerLane = 5;

    private Dictionary<Lane, Queue<VehicleController>> _pool;
    private Dictionary<Lane, float>                    _spawnTimers;

    private void Awake()
    {
        Instance     = this;
        _pool        = new Dictionary<Lane, Queue<VehicleController>>();
        _spawnTimers = new Dictionary<Lane, float>();
    }

    private void Start()
    {
        foreach (Lane lane in lanes)
        {
            _pool[lane]        = new Queue<VehicleController>();
            _spawnTimers[lane] = 0f;

            for (int i = 0; i < poolSizePerLane; i++)
            {
                VehicleController vc = CreateVehicle(lane);
                vc.gameObject.SetActive(false);
                _pool[lane].Enqueue(vc);
            }
        }
    }

    private void Update()
    {
        foreach (Lane lane in lanes)
        {
            _spawnTimers[lane] -= Time.deltaTime;
            if (_spawnTimers[lane] <= 0f)
            {
                SpawnFromPool(lane);
                _spawnTimers[lane] = lane.spawnInterval;
            }
        }
    }

    private void SpawnFromPool(Lane lane)
    {
        if (_pool[lane].Count == 0) return;

        VehicleController vc    = _pool[lane].Dequeue();
        float             speed = Random.Range(lane.minSpeed, lane.maxSpeed);
        vc.ResetToSpawn(lane.spawnPoint.position, speed, lane.trafficLight);
        vc.gameObject.SetActive(true);
    }

    private VehicleController CreateVehicle(Lane lane)
    {
        GameObject prefab = vehiclePrefabs[Random.Range(0, vehiclePrefabs.Length)];
        GameObject go     = Instantiate(prefab, lane.spawnPoint.position,
                                        Quaternion.identity, transform);
        VehicleController vc = go.GetComponent<VehicleController>();
        vc.AssignTrafficLight(lane.trafficLight);
        return vc;
    }

    public void RecycleVehicle(VehicleController vc)
    {
        vc.gameObject.SetActive(false);
        foreach (var pair in _pool) { pair.Value.Enqueue(vc); return; }
    }
}