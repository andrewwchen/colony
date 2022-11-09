using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script that manages a fade effect, and provides methods for creating a smooth fade in/out effect
/// </summary>
public class FadeController : MonoBehaviour
{
    public static FadeController Instance = null;

    [Tooltip("The overlay prefab to use")]
    public GameObject overlayPrefab;
    [Tooltip("Percent to fade to")]
    public float goalPercent = 1;
    [Tooltip("How fast to fade in and out")]
    public float fadeSpeed = 20;
    [Tooltip("fade color")]
    public Color fadeColor = new Color(0, 0, 0);
    // The current fade percent
    [HideInInspector] public float currentPercent = 100;

    // Instance of the overlay prefab
    private GameObject overlayInstance;
    // Overlay's renderer
    private Renderer overlayRenderer;

    // Player head transform
    [HideInInspector] public Transform playerHead;

    // Whether the effect is currently active
    [HideInInspector] public bool active = false;
    // Whether fading or unfading
    private bool fading = false;
   
    // Fade-related events
    public delegate void FadeEvent();
    public event FadeEvent OnFadeStart;     // Called when a fade starts
    public event FadeEvent OnFadeEnd;       // Called when a fade ends
    public event FadeEvent OnUnfadeStart;   // Called when an unfade starts
    public event FadeEvent OnUnfadeEnd;     // Called when an unfade ends

    void Awake()
    {
        if (Instance == null)
            Instance = this; // set instance to this first and only instance of this class
        else if (Instance != this) // as a safety check if this is a new instance of this class
            Destroy(gameObject); // to avoid a duplication and thereby ensure a singleton

        // Get player head transform
        playerHead = GameObject.FindGameObjectWithTag("MainCamera").transform;

        // Start fade off active
        active = true;
        fading = true;

        StartUnfade();
    }

    // Called to start a fade
    public void StartFade(bool skip = false)
    {

        // Make sure fade isn't already started
        if (!active)
        {
            // Invoke the fade start event
            OnFadeStart?.Invoke();
            // Instantiate overlay on player head
            overlayInstance = Instantiate(overlayPrefab, playerHead);
            overlayRenderer = overlayInstance.GetComponent<Renderer>();
            // Set initial fade percent
            UpdateOpacity(currentPercent);
            // Mark effect as active
            active = true;
            // Remember that effect starts with fading
            fading = true;

            // If fade is already complete, immediately end it
            if (currentPercent == goalPercent)
                OnFadeEnd?.Invoke();
        }
    }

    // Called to start an unfade
    public void StartUnfade()
    {
        // Make sure unfade isn't already started
        if (fading)
        {
            // Invoke the unfade start event
            OnUnfadeStart?.Invoke();
            // Mark effect as inactive
            fading = false;
        }
        // If unfade is already complete, immediately end it
        if (currentPercent == 0)
            Stop();
    }

    // Called when application quits
    void OnApplicationQuit()
    {
        // If the effect is currently active, remove it
        if (active)
        {
            Stop();
        }           
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            if (fading)
            {

                // If fade percent is less than the max, increase it
                if (currentPercent < goalPercent)
                {
                    currentPercent += fadeSpeed * Time.deltaTime;
                    if (currentPercent >= goalPercent)
                    {
                        // If fade percent has reached the max, cap it and invoke the event
                        currentPercent = goalPercent;
                        OnFadeEnd?.Invoke();
                    }
                }
            }
            else
            {
                // If fade percent is greater than the min, decrease it
                if (currentPercent > 0)
                {
                    currentPercent -= fadeSpeed * Time.deltaTime;
                    if (currentPercent <= 0)
                    {
                        Stop();
                    }
                }
            }

            // Set material's opacity
            UpdateOpacity(currentPercent);
            // Set overlay rotation
            overlayInstance.transform.rotation = playerHead.rotation;

        }
    }

    public void Stop()
    {
        if (active)
        {
            // Invoke fade end event
            OnUnfadeEnd?.Invoke();
            // Mark the effect as inactive
            active = false;

            // Reset shader's fade percent
            currentPercent = 0;
            UpdateOpacity(currentPercent);
            // Remove the effect
            Destroy(overlayInstance);
        }      
    }

    // Set opacity of overlay
    private void UpdateOpacity(float opacity)
    {
        if (overlayRenderer)
            overlayRenderer.material.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, opacity);
    }
}
