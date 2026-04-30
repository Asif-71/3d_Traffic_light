using UnityEngine;

[ExecuteInEditMode]
public class ZebraStripe : MonoBehaviour
{
    [Header("Stripe Settings")]
    [SerializeField] private int   stripeCount  = 8;
    [SerializeField] private float stripeWidth  = 0.8f;
    [SerializeField] private float stripeGap    = 0.4f;
    [SerializeField] private float roadWidth    = 8f;
    [SerializeField] private float stripeHeight = 0.01f;

    [Header("Materials")]
    [SerializeField] private Material whiteMaterial;
    [SerializeField] private Material darkMaterial;

    [Header("Auto Generate")]
    [SerializeField] private bool generateOnStart = true;

    private void Start()
    {
        if (generateOnStart)
            GenerateStripes();
    }

    [ContextMenu("Generate Stripes")]
    public void GenerateStripes()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);

        float totalStep = stripeWidth + stripeGap;
        float startZ    = -(stripeCount * totalStep) / 2f;

        for (int i = 0; i < stripeCount; i++)
        {
            GameObject stripe = GameObject.CreatePrimitive(PrimitiveType.Cube);
            stripe.name = $"Stripe_{i}";
            stripe.transform.SetParent(transform, false);

            float zPos = startZ + i * totalStep + stripeWidth / 2f;
            stripe.transform.localPosition = new Vector3(0f, stripeHeight, zPos);
            stripe.transform.localScale    = new Vector3(roadWidth, stripeHeight, stripeWidth);

            stripe.GetComponent<Renderer>().material =
                (i % 2 == 0) ? whiteMaterial : darkMaterial;

            Destroy(stripe.GetComponent<Collider>());
        }

        Debug.Log($"[ZebraStripe] Generated {stripeCount} stripes.");
    }
}