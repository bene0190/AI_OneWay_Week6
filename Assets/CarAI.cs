using UnityEngine;

// CarAI is the "context" of the State Pattern.
// It owns the state machine, sensors, movement, and current traffic light (if near).
public class CarAI : MonoBehaviour
{
    [Header("Speeds")]
    public float goSpeed = 12f;    // Target speed when GO state
    public float slowSpeed = 6f;   // Target speed when SLOWDOWN state
    public float accel = 8f;       // How fast we accelerate upward
    public float brake = 14f;      // How fast we decelerate downward

    [Header("Sensors")]
    public float frontCheckDistance = 6f; // Raycast distance to detect a car in front
    public float stopDistance = 2f;       // Distance considered "too close" (must STOP)
    public LayerMask carLayer;            // What layers count as cars

    [Header("References")]
    public Transform sensorOrigin;        // Optional front bumper position for raycast start

    // ActiveTrafficLight is ONLY non-null when inside an IntersectionZone trigger.
    // That means cars far away ignore the light completely.
    public TrafficLight ActiveTrafficLight { get; private set; }

    // CurrentSpeed is the speed we are currently moving at.
    public float CurrentSpeed { get; private set; }

    // CarAheadDetected means raycast saw a car in front (within frontCheckDistance).
    public bool CarAheadDetected { get; private set; }

    // CarAheadStoppedClose means:
    // 1) a car is detected in front AND
    // 2) that car is basically stopped AND
    // 3) it is within stopDistance
    public bool CarAheadStoppedClose { get; private set; }

    // The state machine that runs the current state.
    private StateMachine sm;

    // State instances (created once, reused).
    public CarStopState StopState { get; private set; }
    public CarGoState GoState { get; private set; }
    public CarSlowdownState SlowdownState { get; private set; }

    void Awake()
    {
        // Create the state machine instance.
        sm = new StateMachine();

        // Create the states and pass:
        // - this CarAI context (so states can read sensors and set speed)
        // - the shared state machine (so states can switch states)
        StopState = new CarStopState(this, sm);
        GoState = new CarGoState(this, sm);
        SlowdownState = new CarSlowdownState(this, sm);
    }

    void Start()
    {
        // Choose starting state.
        // Starting STOP is safe so cars won't immediately move before logic is ready.
        sm.Change(StopState);
    }

    void Update()
    {
        // Update sensors each frame (detect car in front).
        UpdateSensors();

        // Run the active state's logic.
        sm.Tick();

        // Move the car forward based on CurrentSpeed.
        MoveForward();
    }

    void UpdateSensors()
    {
        // If sensorOrigin is set (front bumper), use it.
        // Otherwise use the main transform position.
        Transform origin = sensorOrigin != null ? sensorOrigin : transform;

        // Forward direction of the car.
        Vector3 dir = origin.forward;

        // Reset sensor results each frame.
        CarAheadDetected = false;
        CarAheadStoppedClose = false;

        // Draw ray in Scene view so you can debug detection.
        Debug.DrawRay(origin.position, dir * frontCheckDistance);

        // Cast a ray forward to see if a car is in front.
        if (Physics.Raycast(origin.position, dir, out RaycastHit hit, frontCheckDistance, carLayer))
        {
            // We hit something on the car layer.
            CarAheadDetected = true;

            // Try to get the other car's CarAI (to know its speed).
            CarAI other = hit.collider.GetComponentInParent<CarAI>();

            // If other exists, read its speed. Else treat it as 0.
            float otherSpeed = other != null ? other.CurrentSpeed : 0f;

            // Consider other car stopped if speed is almost zero.
            bool otherStopped = otherSpeed <= 0.1f;

            // Consider "very close" if hit is within stopDistance.
            bool veryClose = hit.distance <= stopDistance;

            // If the other car is stopped and close, we must stop too.
            CarAheadStoppedClose = otherStopped && veryClose;
        }
    }

    // Smoothly changes CurrentSpeed toward a target speed.
    public void SetTargetSpeed(float target)
    {
        // If target is higher than current -> accelerate.
        // If target is lower than current -> brake.
        float rate = (target > CurrentSpeed) ? accel : brake;

        // MoveTowards makes smooth change without overshoot.
        CurrentSpeed = Mathf.MoveTowards(CurrentSpeed, target, rate * Time.deltaTime);
    }

    // Moves the car forward using transform.forward.
    // This is simple movement (not physics-based).
    void MoveForward()
    {
        transform.position += transform.forward * (CurrentSpeed * Time.deltaTime);
    }

    // Called by IntersectionZone when the car ENTERS the zone.
    public void SetActiveTrafficLight(TrafficLight light)
    {
        // Store reference so states can read it.
        ActiveTrafficLight = light;
    }

    // Called by IntersectionZone when the car EXITS the zone.
    public void ClearActiveTrafficLight(TrafficLight light)
    {
        // Only clear if it matches current assigned light.
        // This prevents other zones from accidentally clearing.
        if (ActiveTrafficLight == light)
            ActiveTrafficLight = null;
    }
}
