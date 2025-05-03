using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// LightMap is responsible for mapping brainwave data to light properties.
/// It adjusts the color and intensity of lights based on the sent brainwave values.
/// The mapping is based on research findings regarding the effects of different 
/// brainwave states on human perception and mood.
/// </summary>
public class LightMap : MonoBehaviour
{
    [System.Serializable]
    /// <summary>
    /// Defines the mapping between a light and a specific brainwave type.
    /// It includes settings for threshold values, colors for low and high states,
    /// and advanced settings for smooth transitions.
    /// </summary>
    public class BrainwaveLightMapping // class within a class (scary)
    {
        public Light light;
        public BrainwaveType brainwaveType;
        
        [Header("Threshold Settings")]
        [Range(0f, 50f)] public float thresholdValue = 5f; // value above which the brainwave is considered "high"
        
        [Header("Low State Colors")]
        public Color lowStateColor;
        [Range(0f, 100f)] public float lowStateIntensity = 0.5f;
        
        [Header("High State Colors")]
        public Color highStateColor;
        [Range(0f, 100f)] public float highStateIntensity = 3f;
        
        [Header("Advanced Settings")]
        [Range(0f, 5f)] public float transitionBuffer = 1f; // range around threshold for smooth transition
        public bool usePresetColors = true;
    }

    public enum BrainwaveType // enum to categorize brainwave types
    {
        Gamma,
        Alpha,
        Beta,
        Theta
    }

    [SerializeField] private static BrainwaveLightMapping[] lightMappings;
    
    [Header("Smoothing")]
    [SerializeField] private static bool useSmoothing = true; // whether to smooth the transitions
    [Range(0.01f, 5f)] [SerializeField] private static float smoothingSpeed = 1.0f;
    
    // store current HSV values for smoothing
    private static Dictionary<Light, Color> targetColors = new Dictionary<Light, Color>();
    private static Dictionary<Light, float> targetIntensities = new Dictionary<Light, float>();

    private void Start()
    {
        // Initialize dictionaries for smoothing
        foreach (var mapping in lightMappings)
        {
            if (mapping.light != null)
            {
                // set default research-based colors if using presets
                if (mapping.usePresetColors)
                {
                    PresetColors(mapping);
                }
                
                targetColors[mapping.light] = mapping.light.color;
                targetIntensities[mapping.light] = mapping.light.intensity;
            }
        }
    }

    /// <summary>
    /// Sets the colors for each brainwave type based on research findings.
    /// This method assigns specific colors to high and low states of brainwave activity.
    /// </summary>
    /// <param name="mapping">The BrainwaveLightMapping object containing the brainwave type and colors.</param>
    /// <remarks>
    /// The colors are chosen based on the effects of different brainwave states on human perception and mood.
    /// </remarks>
    private void PresetColors (BrainwaveLightMapping mapping)
    {
        switch (mapping.brainwaveType)
        {
            case BrainwaveType.Theta:
                // theta high = pink, orange, bright colors
                // theta low = dark greys, blacks
                mapping.highStateColor = new Color(1.0f, 0.5f, 0.8f); // bright pink/orange
                mapping.lowStateColor = new Color(0.2f, 0.2f, 0.2f);  // dark grey
                break;
            case BrainwaveType.Alpha:
                // alpha high = calming green, blues
                // alpha low = red, magenta
                mapping.highStateColor = new Color(0.0f, 0.8f, 0.8f); // calming blue-green
                mapping.lowStateColor = new Color(1.0f, 0.0f, 0.5f);  // magenta/red
                break;
            case BrainwaveType.Beta:
                // beta high = blue green, green
                // beta low = darker muted colors
                mapping.highStateColor = new Color(0.0f, 0.8f, 0.5f); // blue-green
                mapping.lowStateColor = new Color(0.7f, 0.2f, 0.2f);  // darker muted color (?)
                break;
            case BrainwaveType.Gamma:
                // gamma high = bright white, yellow
                // gamma low = gray, dark colors
                mapping.highStateColor = new Color(1.0f, 1.0f, 1.0f); // white
                mapping.lowStateColor = new Color(0.5f, 0.5f, 0.5f);  // gray
                break;
        }
    }

    private static void Update()
    {
        foreach (var mapping in lightMappings)
        {
            if (mapping.light == null) continue;
            
            float brainwaveValue = GetBrainwaveValue(mapping.brainwaveType);
            
            Color targetColor;
            float targetIntensity;
            
            // determine if we're in high or low state + smooth transition
            if (brainwaveValue > mapping.thresholdValue + mapping.transitionBuffer)
            {
                // high state
                targetColor = mapping.highStateColor;
                targetIntensity = mapping.highStateIntensity;
            }
            else if (brainwaveValue < mapping.thresholdValue - mapping.transitionBuffer)
            {
                // low state
                targetColor = mapping.lowStateColor;
                targetIntensity = mapping.lowStateIntensity;
            }
            else
            {
                // transition zone - blend between low and high
                float t = (brainwaveValue - (mapping.thresholdValue - mapping.transitionBuffer)) / (2 * mapping.transitionBuffer);
                targetColor = Color.Lerp(mapping.lowStateColor, mapping.highStateColor, t);
                targetIntensity = Mathf.Lerp(mapping.lowStateIntensity, mapping.highStateIntensity, t);
            }
            
            // update the target values
            targetColors[mapping.light] = targetColor;
            targetIntensities[mapping.light] = targetIntensity;
            
            // apply colors (with or without smoothing)
            if (useSmoothing)
            {
                mapping.light.color = Color.Lerp(mapping.light.color, targetColor, Time.deltaTime * smoothingSpeed);
                mapping.light.intensity = Mathf.Lerp(mapping.light.intensity, targetIntensity, Time.deltaTime * smoothingSpeed);
            }
            else
            {
                mapping.light.color = targetColor;
                mapping.light.intensity = targetIntensity;
            }
        }
    }

    /// <summary>
    /// This method accesses the EEG_Receiver to get the latest brainwave data.
    /// It returns the value for the specified brainwave type, which is used to determine
    /// the light properties.
    /// </summary>
    /// <param name="type">The type of brainwave to retrieve the value for.</param>
    /// <returns>The current value of the specified brainwave type.</returns>
    private static float GetBrainwaveValue(BrainwaveType type)
    {
        switch (type)
        {
            case BrainwaveType.Gamma:
                return EEG_Receiver.gamma_value;
            case BrainwaveType.Alpha:
                return EEG_Receiver.alpha_value;
            case BrainwaveType.Beta:
                return EEG_Receiver.beta_value;
            case BrainwaveType.Theta:
                return EEG_Receiver.theta_value;
            default:
                return 0f;
        }
    }
}