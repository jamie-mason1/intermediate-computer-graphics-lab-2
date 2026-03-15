using UnityEngine;

public class FlashingChargingEmission : MonoBehaviour
{
    [Header("Escalation Settings")]
    [SerializeField] float escalationSpeed = 0.2f;   // How fast it builds over time
    [SerializeField] float pulseSpeed = 2f;          // Flash speed
    [SerializeField] float maxIntensity = 8f;        // Max emission intensity
    [SerializeField] float brightnessBoost = 2f;     // How bright value can get

    Renderer rend;
    Material mat;

    float hue;
    float saturation;
    float baseValue;

    float escalationTimer;

    void Start()
    {
        rend = GetComponent<Renderer>();
        mat = rend.material;

        // Enable emission
        mat.EnableKeyword("_EMISSION");

        // Get base emission color
        Color baseEmission = mat.GetColor("_EmissionColor");

        // Convert to HSV so we can preserve hue + saturation
        Color.RGBToHSV(baseEmission, out hue, out saturation, out baseValue);

        escalationTimer = 0f;
    }

    void Update()
    {
        // 1️⃣ Build escalation over time (0 → 1 → 0 loop)
        escalationTimer += Time.deltaTime * escalationSpeed;
        float escalation = Mathf.PingPong(escalationTimer, 1f);

        // 2️⃣ Add pulsing on top
        float pulse = Mathf.Sin(Time.time * pulseSpeed) * 0.5f + 0.5f;

        // Pulse strength increases as escalation increases
        float combined = pulse * escalation;

        // 3️⃣ Adjust brightness (Value only)
        float newValue = Mathf.Clamp01(baseValue * (1f + combined * brightnessBoost));

        Color newColor = Color.HSVToRGB(hue, saturation, newValue);

        mat.SetColor("_EmissionColor", newColor);

        // 4️⃣ Adjust emission intensity
        float intensity = combined * maxIntensity;
        mat.SetFloat("_EmissionIntensity", intensity);
    }
}