namespace KitchenChaos;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

[Meta, StateDiagram]
public partial record TrashCounterLogicState : LogicBlockState
{
  [Meta, Id("trash_counter_logic_state_empty")]
  public partial record Empty : TrashCounterLogicState,
    IGet<Input.Interact>
  {
    public Type On(in Input.Interact input)
    {
      if (!input.PlayerHasKitchenObject)
      {
        Output(new Output.InteractionRejected());
        return ToSelf();
      }

      Output(new Output.PlaceRequested());
      return ToSelf();
    }
  }

  public static class Input
  {
    public readonly record struct Interact(
      bool PlayerHasKitchenObject
    );
  }

  public static class Output
  {
    public readonly record struct PlaceRequested;
    public readonly record struct InteractionRejected;
  }
}
