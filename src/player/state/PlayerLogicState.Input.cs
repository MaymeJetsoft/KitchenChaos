namespace KitchenChaos;

using Godot;

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
    public readonly record struct FacingCounterChanged(ICounter? Counter);
    public readonly record struct InteractionStarted(ICounter Counter);
    public readonly record struct InteractionCompleted(ICounter Counter);
    public readonly record struct PickUpCompleted(ICounter Counter);
  }
}
