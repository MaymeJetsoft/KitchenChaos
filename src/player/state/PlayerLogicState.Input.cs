namespace ChickenChaos;

using Godot;
using KitchenChaos;

public abstract partial record PlayerLogicState
{
  public static class Input
  {
    public readonly record struct Enable;
    public readonly record struct PhysicsTick(double Delta);
    public readonly record struct Moved(Vector3 GlobalPosition);
    public readonly record struct Pushed(Vector3 GlobalForceImpulseVector);
    public readonly record struct StartedMovingHorizontally;
    public readonly record struct StoppedMovingHorizontally;
    public readonly record struct Killed;
    public readonly record struct CounterChanged(ICounter? Counter);
    public readonly record struct InteractPressed(ICounter Counter);

    // public readonly record struct StartedFalling;
    // public readonly record struct HitFloor(bool IsMovingHorizontally);
    // public readonly record struct LeftFloor(bool IsFalling);
    // public readonly record struct Jump(double Delta);
  }
}
