using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public float dayDuration = 60f; // Total duration of a day in seconds
    public float nightSpeedMultiplier = 5f; // Multiplier for the night speed
    private float dayFraction;
    private float anglePerSecond;

    private Light sunLight;
    private float initialIntensity;

    void Start()
    {
        sunLight = GetComponent<Light>();
        initialIntensity = sunLight.intensity;

        // Calculate the angle to rotate per second for a full cycle
        anglePerSecond = 360f / dayDuration;
    }

    void Update()
    {
        // Calculate the current time of day (0 = start of day, 1 = end of day)
        dayFraction = (Time.time % dayDuration) / dayDuration;

        float rotationSpeed = anglePerSecond;

        if (dayFraction > 0.25f && dayFraction < 0.75f) // Daytime (6 AM - 6 PM)
        {
            sunLight.intensity = initialIntensity;
        }
        else // Nighttime (6 PM - 6 AM)
        {
            rotationSpeed *= nightSpeedMultiplier;
            sunLight.intensity = Mathf.Lerp(0, initialIntensity, Mathf.InverseLerp(0.75f, 0.25f, dayFraction));
        }

        // Apply rotation based on the calculated speed
        transform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime);
    }
}