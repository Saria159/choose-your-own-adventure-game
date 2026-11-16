// A die that returns preset rolls in order, so tests control randomness.
public class FakeDie : Die
{
    private readonly Queue<int> _rolls;

    public FakeDie(params int[] rolls)
    {
        _rolls = new Queue<int>(rolls);
    }

    public int RemainingRolls => _rolls.Count;

    public override int Roll(int sides)
    {
        if (_rolls.Count == 0)
            throw new InvalidOperationException($"FakeDie ran out of scripted rolls (asked for d{sides}).");

        return _rolls.Dequeue();
    }
}
