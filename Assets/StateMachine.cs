// StateMachine is responsible for:
// 1) Storing the current state
// 2) Switching states safely (Exit -> Enter)
// 3) Updating the current state every frame (Tick)
public class StateMachine
{
    // Current holds the active state object.
    // private set = only this class can change it.
    public ICarState Current { get; private set; }

    // Change switches the active state to "next".
    public void Change(ICarState next)
    {
        // If next is null, do nothing (avoid crashes).
        // If next is the same as current, do nothing (avoid repeated Enter calls).
        if (next == null || next == Current) return;

        // If there is a current state, call Exit() before switching.
        // ?. means "call only if not null".
        Current?.Exit();

        // Set the new state.
        Current = next;

        // Call Enter() once when new state begins.
        Current.Enter();
    }

    // Tick calls the active state's Tick() every frame.
    public void Tick()
    {
        // Call Tick only if Current is not null.
        Current?.Tick();
    }
}
