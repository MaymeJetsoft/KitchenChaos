namespace KitchenChaos;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

[Meta, StateDiagram]
public partial record ClearCounterLogicState : LogicBlockState,
  IGet<ClearCounterLogicState.Input.Interact>
{
  public static class Input
  {
    public readonly record struct Interact(
      bool PlayerHasKitchenObject,
      bool CounterHasKitchenObject
    );
  }

  public static class Output
  {
    public readonly record struct PlaceRequested;
    public readonly record struct TakeRequested;
    public readonly record struct InteractionRejected;
  }

  public Type On(in Input.Interact input)
  {
    if (!input.CounterHasKitchenObject && input.PlayerHasKitchenObject)
    {
      Output(new Output.PlaceRequested());
    }
    else if (input.CounterHasKitchenObject && !input.PlayerHasKitchenObject)
    {
      Output(new Output.TakeRequested());
    }
    else
    {
      Output(new Output.InteractionRejected());
    }

    return ToSelf();
  }
}