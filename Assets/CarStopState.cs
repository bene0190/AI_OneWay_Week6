// STOP state:
// - Brakes to 0 speed
// - Stays stopped if light is RED (only when near) OR front car stopped close
// - Otherwise switches to Slowdown or Go
public class CarStopState : ICarState
{
    // Reference to the car context (sensors + movement).
    private readonly CarAI car;

    // Reference to state machine so we can switch states.
    private readonly StateMachine sm;

    // Constructor receives context and machine.
    public CarStopState(CarAI car, StateMachine sm)
    {
        this.car = car;
        this.sm = sm;
    }

    // Called once when entering STOP.
    public void Enter() { }

    // Called every frame while in STOP.
    public void Tick()
    {
        // Always target speed 0 in STOP.
        car.SetTargetSpeed(0f);

        // Light is only considered if ActiveTrafficLight is not null (near intersection).
        bool red = car.ActiveTrafficLight != null && car.ActiveTrafficLight.IsRed;

        // If red light OR front car is stopped close, keep stopping.
        if (red || car.CarAheadStoppedClose) return;

        // If orange light while near intersection, we should not go full speed.
        bool orange = car.ActiveTrafficLight != null && car.ActiveTrafficLight.IsOrange;

        // If orange OR there is a car ahead, use Slowdown.
        if (orange || car.CarAheadDetected)
        {
            sm.Change(car.SlowdownState);
        }
        else
        {
            // Otherwise safe to go.
            sm.Change(car.GoState);
        }
    }

    // Called once when leaving STOP.
    public void Exit() { }
}
