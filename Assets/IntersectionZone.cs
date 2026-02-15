using UnityEngine;

// RequireComponent forces the GameObject to have a Collider.
// This prevents forgetting to add one.
[RequireComponent(typeof(Collider))]
public class IntersectionZone : MonoBehaviour
{
    // The TrafficLight that this zone controls.
    // When a car enters this zone, it will obey THIS light.
    public TrafficLight trafficLight;

    // Reset is called when you add the component in the editor.
    // Helpful to auto-set the collider as a trigger.
    void Reset()
    {
        // Get the collider on this object and make it a trigger.
        // Trigger = detects overlaps without physical collision response.
        GetComponent<Collider>().isTrigger = true;
    }

    // Called when something enters this trigger volume.
    void OnTriggerEnter(Collider other)
    {
        // Try to find a CarAI component on this object or its parents.
        // Using GetComponentInParent allows colliders on child objects.
        CarAI car = other.GetComponentInParent<CarAI>();

        // If the entering object is a car...
        if (car != null)
        {
            // Give the car a reference to this traffic light.
            // ✅ This is what makes ONLY nearby cars obey the light.
            car.SetActiveTrafficLight(trafficLight);
        }
    }

    // Called when something exits this trigger volume.
    void OnTriggerExit(Collider other)
    {
        // Again, try to find the car controller.
        CarAI car = other.GetComponentInParent<CarAI>();

        // If it's a car...
        if (car != null)
        {
            // Remove the reference ONLY if it matches this light.
            // ✅ Car is no longer nearby, so it ignores the light again.
            car.ClearActiveTrafficLight(trafficLight);
        }
    }
}
