// This interface defines the "contract" for every car state.
// Any state (Stop/Go/Slowdown) MUST have these 3 functions.
public interface ICarState
{
    void Enter();  // Called ONCE when switching INTO this state
    void Tick();   // Called EVERY FRAME while this state is active
    void Exit();   // Called ONCE when switching OUT of this state
}
