using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    public enum LightState { Red, Amber, Green }

    [Header("Timing (seconds)")]
    [SerializeField] private float greenDuration  = 5f;
    [SerializeField] private float amberDuration  = 2f;
    [SerializeField] private float redDuration    = 6f;
    [SerializeField] private LightState startState = LightState.Red;

    [Header("Light Objects - drag the actual light GameObjects here")]
    [SerializeField] private GameObject redLightObject;
    [SerializeField] private GameObject amberLightObject;
    [SerializeField] private GameObject greenLightObject;

    [Header("Light Colours")]
    [SerializeField] private Color redColour   = new Color(1f, 0f, 0f);
    [SerializeField] private Color amberColour = new Color(1f, 0.65f, 0f);
    [SerializeField] private Color greenColour = new Color(0f, 1f, 0f);
    [SerializeField] private Color offColour   = new Color(0.1f, 0.1f, 0.1f);

    [Header("Audio (optional)")]
    [SerializeField] private AudioSource beepSource;

    public LightState CurrentState { get; private set; }
    public event System.Action<LightState> OnStateChanged;

    private float _timer;

    private void Start()
    {
        CurrentState = startState;
        _timer = GetDurationForState(CurrentState);
        ApplyLightVisuals();
    }

    private void Update()
    {
        _timer -= Time.deltaTime;
        if (_timer <= 0f)
            AdvanceState();
    }

    private void AdvanceState()
    {
        CurrentState = CurrentState switch
        {
            LightState.Red   => LightState.Green,
            LightState.Green => LightState.Amber,
            LightState.Amber => LightState.Red,
            _                => LightState.Red
        };

        _timer = GetDurationForState(CurrentState);
        ApplyLightVisuals();
        OnStateChanged?.Invoke(CurrentState);

        if (CurrentState == LightState.Green && beepSource != null)
            beepSource.Play();
    }

    private float GetDurationForState(LightState state) => state switch
    {
        LightState.Green => greenDuration,
        LightState.Amber => amberDuration,
        LightState.Red   => redDuration,
        _                => redDuration
    };

    private void ApplyLightVisuals()
    {
        // Set each light colour based on current state
        SetLightColour(redLightObject,   
            CurrentState == LightState.Red   ? redColour   : offColour);

        SetLightColour(amberLightObject, 
            CurrentState == LightState.Amber ? amberColour : offColour);

        SetLightColour(greenLightObject, 
            CurrentState == LightState.Green ? greenColour : offColour);
    }

    private void SetLightColour(GameObject lightObj, Color colour)
    {
        if (lightObj == null) return;

        Renderer rend = lightObj.GetComponent<Renderer>();
        if (rend == null) return;

        // Create a new material instance so we dont affect the original
        Material mat = rend.material;
        mat.color = colour;

        // Enable emission so the light glows
        if (colour != offColour)
        {
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", colour * 2f);
        }
        else
        {
            mat.DisableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", Color.black);
        }
    }

    public bool IsSafeForPedestrian() => CurrentState == LightState.Green;
    public float GetRemainingTime()   => _timer;
}