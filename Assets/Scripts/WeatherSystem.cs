using UnityEngine;
using UnityEngine.Rendering.Universal;

public class WeatherSystem : MonoBehaviour
{
    public Light2D EnvironmentLight; 
    public static bool isDay;        

    [Header("Day/Night Cycle Settings")]
    public float dayDuration = 60f;        
    public float minLightIntensity = 0.1f; 
    public float maxLightIntensity = 1.0f; 
    public int startDay = 1;               

    [Header("Tracked Time")]
    public int currentDay;                
    private float currentTime ;             

    void Start()
    {
        
        currentDay = startDay;
        currentTime = 20f;
    }

    void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime > dayDuration)
        {
            currentTime = 0f;
            currentDay++;
            Debug.Log($"A new day has begun! Day: {currentDay}");
        }

        float normalizedTime = currentTime / dayDuration;
        float lightIntensity = Mathf.Lerp(minLightIntensity, maxLightIntensity, Mathf.Sin(normalizedTime * Mathf.PI));
        EnvironmentLight.intensity = lightIntensity;
        isDay = lightIntensity > (minLightIntensity + maxLightIntensity) / 2f;
    }
}
