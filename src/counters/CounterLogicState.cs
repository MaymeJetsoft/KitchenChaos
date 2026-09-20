namespace KitchenChaos;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

[Meta, StateDiagram]
public partial record CounterLogicState : LogicBlockState
{
  public static class Output
  {
    public readonly record struct FacingCounterChanged(
      ICounter? Counter
    );

    public readonly record struct CounterInteracted(
      ICounter Counter
    );

    public readonly record struct KitchenObjectPickedUp(
      ICounter Counter
    );
  }
}
