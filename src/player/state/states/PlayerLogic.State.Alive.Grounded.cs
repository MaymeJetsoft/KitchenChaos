namespace ChickenChaos;

using Chickensoft.Introspection;

public abstract partial record PlayerLogicState
{
  [Meta]
  public abstract partial record Grounded : Alive { }
}
