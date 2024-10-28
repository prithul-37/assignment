using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class WeatherSystem : MonoBehaviour
{   

    public Light2D EnvironmentLight;
    public static bool isDay;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   

        EnvironmentLight.intensity = Mathf.Abs(Mathf.Cos(Time.time/30));

        if(EnvironmentLight.intensity > .3f) isDay = true;
        else isDay = false;
    }
}
