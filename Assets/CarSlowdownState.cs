// SLOWDOWN state:
// - Drives at slowSpeed
// - Switches to STOP if red light (near) OR front stopped close
// - Switches to GO if conditions are clear and light is green (near) OR no light (not near)
public class CarSlowdownState : ICarState
{
    private readonly CarAI car;
    private readonly StateMachine sm;

    public CarSlowdownState(CarAI car, StateMachine sm)
    {
        this.car = car;
        this.sm = sm;
    }

    public void Enter() { }

    public void Tick()
    {
        // If near a light and it turns red, stop.
        bool red = car.ActiveTrafficLight != null && car.ActiveTrafficLight.IsRed;

        // If red OR front car stopped close, switch to STOP immediately.
        if (red || car.CarAheadStoppedClose)
        {
            sm.Change(car.StopState);
            return;
        }

        // Maintain slower speed.
        car.SetTargetSpeed(car.slowSpeed);

        // If near the light and it's green, we can speed up if clear.
        bool green = car.ActiveTrafficLight != null && car.ActiveTrafficLight.IsGreen;

        // If no car ahead and light is green, go back to GO.
        if (!car.CarAheadDetected && green)
        {
            sm.Change(car.GoState);
            return;
        }

        // If NOT in an intersection zone (ActiveTrafficLight == null),
        // ignore the light and go fast when road is clear.
        if (car.ActiveTrafficLight == null && !car.CarAheadDetected)
        {
            sm.Change(car.GoState);
        }
    }

    public void Exit() { }
}
