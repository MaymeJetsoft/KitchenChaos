namespace ChickenChaos;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using KitchenChaos;

[Meta, StateDiagram]
public partial record CounterLogicState : LogicBlockState
{
  public static class Output
  {
    public readonly record struct InteractableCounterChanged(
      ICounter? InteractableCounter
    );

    public readonly record struct CurrentCounterChanged(
      ICounter Counter
    );
  }
}
