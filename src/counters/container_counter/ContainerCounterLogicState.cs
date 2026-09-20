namespace KitchenChaos;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

[Meta, StateDiagram]
public partial record ContainerCounterLogicState : LogicBlockState,
  IGet<ContainerCounterLogicState.Input.Interact>
{
  public static class Input
  {
    public readonly record struct Interact(bool PlayerHasKitchenObject, bool CounterHasKitchenObject);
  }

  public static class Output
  {
    public readonly record struct SpawnRequested(KitchenObjectType Type);
    public readonly record struct InteractionRejected;
    public readonly record struct TakeRequested;
  }

  public Type On(in Input.Interact input)
  {
    if (input.PlayerHasKitchenObject)
    {
      Output(new Output.InteractionRejected());
    }
    else if (input.CounterHasKitchenObject && !input.PlayerHasKitchenObject)
    {
      Output(new Output.TakeRequested());
    }
    else
    {
      Output(new Output.SpawnRequested(Get<ContainerCounterLogic.Data>().Type));
    }

    return ToSelf();
  }
}
