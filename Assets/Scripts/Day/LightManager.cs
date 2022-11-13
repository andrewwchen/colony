using System.Collections;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    // the LightingSet defining the daytime lighting state
    public LightingSet daySet;
    // the LightingSet defining the nighttime lighting state
    public LightingSet nightSet;
    // number of seconds in one day
    public float dayLengthSeconds = 30;
    // the time in seconds during the at which the sunrise transition starts
    public float sunriseTimeSeconds = 5f;
    // the time in seconds during the at which the sunset transition starts
    public float sunsetTimeSeconds = 20f;
    // the duration in seconds of a sunrise transition
    public float sunriseLengthSeconds = 2f;
    // the duration in seconds of a sunset transition
    public float sunsetLengthSeconds = 2f;

    // the skybox whose properties will be modified during lighting transitions
    private Material skybox;
    // the directional light whose properties will be modified during lighting transitions
    private Light sun;
    // daytime lightmaps packaged into this LightmapData array
    private LightmapData[] dayLightmap;
    // nighttime lightmaps packaged into this LightmapData array
    private LightmapData[] nightLightmap;
    // current time in the day in seconds
    private float timeSeconds = 0;
    // keeps track of the current lighting state
    private bool isNight = true;

    // Start is called before the first frame update
    void Start()
    {
        // get references to the directional light and skybox material to modify during lighting transitions
        skybox = RenderSettings.skybox;
        sun = RenderSettings.sun;

        // package lightmaps into LightmapData arrays to swap during lighting transitions
        dayLightmap = new LightmapData[daySet.lightmapColor.Length];
        for (int i = 0; i < daySet.lightmapColor.Length; i++)
        {
            LightmapData ld = new LightmapData();
            ld.lightmapColor = daySet.lightmapColor[i];
            ld.lightmapDir = daySet.lightmapDir[i];
            dayLightmap[i] = ld;
        }

        nightLightmap = new LightmapData[nightSet.lightmapColor.Length];
        for (int i = 0; i < nightSet.lightmapColor.Length; i++)
        {
            LightmapData ld = new LightmapData();
            ld.lightmapColor = nightSet.lightmapColor[i];
            ld.lightmapDir = nightSet.lightmapDir[i];
            nightLightmap[i] = ld;
        }

        // Set the initial lighting state to nighttime
        LerpLightingSet(true, 1);
    }

    // Update is called once per frame
    void Update()
    {
        timeSeconds += Time.deltaTime;
        timeSeconds = timeSeconds % dayLengthSeconds;

        if (timeSeconds > sunsetTimeSeconds)
        {
            if (!isNight)
            {
                isNight = true;

                // stop ongoing lighting transitions, if any
                StopAllCoroutines();

                // start a sunset lighting transition
                StartCoroutine(TransitionLightingSet(true));
            }
        }
        else if (timeSeconds > sunriseTimeSeconds)
        {
            if (isNight)
            {
                isNight = false;

                // stop ongoing lighting transitions, if any
                StopAllCoroutines();

                // start a sunrise lighting transition
                StartCoroutine(TransitionLightingSet(false));
            }
        }
    }

    // coroutine which handles a lighting transition over the course of its specified duration
    private IEnumerator TransitionLightingSet(bool isSunset)
    {
        float startTime = Time.time;
        float elapsedTime;
        float transitionLength = isSunset ? sunsetLengthSeconds : sunriseLengthSeconds;
        while ((elapsedTime = Time.time - startTime) < transitionLength)
        {
            LerpLightingSet(isSunset, elapsedTime / transitionLength);
            yield return new WaitForFixedUpdate();
        }

        // make sure that the end state of the lighting transition is reached
        LerpLightingSet(isSunset, 1);
    }

    // performs multiple lerps for directional light properties and skybox materials properties
    // swaps the lightmaps halfway through the transition
    private void LerpLightingSet(bool isSunset, float t)
    {
        // change which lighting set we are transitioning to depending on whether it is sunset or sunrise
        LightingSet set1 = isSunset ? daySet : nightSet;
        LightingSet set2 = isSunset ? nightSet : daySet;
        LightmapData[] map1 = isSunset ? dayLightmap : nightLightmap;
        LightmapData[] map2 = isSunset ? nightLightmap : dayLightmap;

        // Lerp sun intensity, angle, and color
        sun.intensity = Mathf.Lerp(set1.light.intensity, set2.light.intensity, t);
        sun.transform.rotation = Quaternion.Lerp(set1.light.transform.rotation, set2.light.transform.rotation, t);
        sun.color = Color.Lerp(set1.light.color, set2.light.color, t);

        // Lerp skybox material properties
        skybox.Lerp(set1.skyboxMat, set2.skyboxMat, t);

        // Swap lightmaps after reaching threshold
        // Lightmaps must be swapped rather than Lerped as Lerping would require iterating over every pixel on
        // each lightmap every frame on CPU in order to Lerp between each pixel color individually.
        // Unity unfortunately does not provide a way of doing this more efficiently
        if (t < 0.5f)
            LightmapSettings.lightmaps = map1;
        else
            LightmapSettings.lightmaps = map2;
    }
}