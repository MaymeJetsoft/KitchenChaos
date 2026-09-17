namespace KitchenChaos;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

[Meta, StateDiagram]
public partial record InGameUILogicState : LogicBlockState
{
  public static class Output
  {
    public readonly record struct FacingCounterChanged(
      ICounter? Counter
    );
    public readonly record struct CurrentCounterChanged(
      ICounter Counter
    );
  }
}
