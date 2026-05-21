namespace RealBhopCS2.Movement;

public readonly record struct MovementVector(float X, float Y, float Z)
{
    public float HorizontalLength => MathF.Sqrt((X * X) + (Y * Y));
}
