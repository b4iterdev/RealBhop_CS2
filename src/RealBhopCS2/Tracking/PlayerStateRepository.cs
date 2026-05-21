using RealBhopCS2.Movement;

namespace RealBhopCS2.Tracking;

public sealed class PlayerStateRepository
{
    private readonly Dictionary<int, PlayerBhopState> _states = new();

    public int Count => _states.Count;

    public PlayerBhopState GetOrCreate(int playerSlot)
    {
        if (_states.TryGetValue(playerSlot, out var state))
        {
            return state;
        }

        state = new PlayerBhopState();
        _states[playerSlot] = state;
        return state;
    }

    public bool Remove(int playerSlot)
    {
        return _states.Remove(playerSlot);
    }

    public void ResetAll()
    {
        _states.Clear();
    }
}
