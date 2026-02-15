// GO state:
// - Drives at goSpeed
// - Switches to STOP if red light (near intersection) OR front car stopped close
// - Switches to SLOWDOWN if orange light (near intersection) OR car detected ahead
public class CarGoState : ICarState
{
    private readonly CarAI car;
    private readonly StateMachine sm;

    public CarGoState(CarAI car, StateMachine sm)
    {
        this.car = car;
        this.sm = sm;
    }

    public void Enter() { }

    public void Tick()
    {
        // If we are near a light and it's red, we must stop.
        bool red = car.ActiveTrafficLight != null && car.ActiveTrafficLight.IsRed;

        // If red OR close stopped car ahead, switch to STOP.
        if (red || car.CarAheadStoppedClose)
        {
            sm.Change(car.StopState);
            return;
        }

        // If near a light and it's orange, slow down.
        bool orange = car.ActiveTrafficLight != null && car.ActiveTrafficLight.IsOrange;

        // If orange OR car detected ahead, switch to SLOWDOWN.
        if (orange || car.CarAheadDetected)
        {
            sm.Change(car.SlowdownState);
            return;
        }

        // Otherwise drive at go speed.
        car.SetTargetSpeed(car.goSpeed);
    }

    public void Exit() { }
}
