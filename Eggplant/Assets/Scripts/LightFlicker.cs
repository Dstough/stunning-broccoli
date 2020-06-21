using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    public float delay = 0f;
    public float speed = 0f;
    public float duration = 0f;

    private float originalDelay;
    private float originalSpeed;
    private float originalDuration;
    private bool lightFlickering;

    private void Awake()
    {
        originalDelay = delay;
        originalSpeed = speed;
        originalDuration = duration;
        lightFlickering = false;
    }

    void Update()
    {
        if (originalDelay == 0)
            return;

        delay -= Time.deltaTime;

        if (lightFlickering)
        {
            speed -= Time.deltaTime;
            duration -= Time.deltaTime;

            if (speed <= 0.0f)
            {
                speed = originalSpeed;
                var light = GetComponent<Light>();
                if (light != null)
                    light.enabled = !light.enabled;
            }
        }

        if (delay <= 0.0f)
        {
            delay = originalDelay;
            lightFlickering = true;
        }

        if (duration <= 0.0f)
        {
            duration = originalDuration;
            lightFlickering = false;
        }

        if (speed <= 0.0f)
            speed = originalSpeed;
    }
}