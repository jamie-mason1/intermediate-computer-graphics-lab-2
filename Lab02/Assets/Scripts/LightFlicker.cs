using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]  // Runs in Play Mode AND Edit Mode
public class LightFlicker : MonoBehaviour
{
     [Header("Pulse Settings")]
    public float minIntensity = 0.5f;
    public float maxIntensity = 2f;
    public float pulseSpeed = 1f;   // How fast it dims in/out

    private Light lightComponent;
    private float timeOffset;

    void Awake()
    {
        lightComponent = GetComponent<Light>();

        // Optional: random start time so multiple lights aren't synced
        timeOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        float pulse = Mathf.Sin((Time.time + timeOffset) * pulseSpeed);

        // Convert sine (-1 to 1) into 0 to 1
        pulse = (pulse + 1f) * 0.5f;

        // Lerp between min and max intensity
        lightComponent.intensity = Mathf.Lerp(minIntensity, maxIntensity, pulse);
    }
}
