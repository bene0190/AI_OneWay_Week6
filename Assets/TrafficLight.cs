using UnityEngine;

// Enum representing possible traffic light colors.
// Using enum makes the Inspector dropdown simple and safe.
public enum LightColor { Red, Orange, Green }

// TrafficLight is a simple component that stores a light color.
// Cars will read these booleans to decide what to do.
public class TrafficLight : MonoBehaviour
{
    // Current light state shown in Inspector.
    // You can manually change it while testing.
    public LightColor current = LightColor.Red;

    // Convenience properties so code reads naturally:
    // e.g. if (light.IsRed) ...
    public bool IsRed => current == LightColor.Red;
    public bool IsOrange => current == LightColor.Orange;
    public bool IsGreen => current == LightColor.Green;
}
