namespace KitchenChaos;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

[Meta, StateDiagram]
public partial record CuttingCounterLogicState : LogicBlockState
{
  [Meta, Id("cutting_counter_logic_state_empty")]
  public partial record Empty : CuttingCounterLogicState,
    IGet<Input.Interact>
  {
    public Type On(in Input.Interact input)
    {
      if (!input.PlayerHasKitchenObject)
      {
        Output(new Output.InteractionRejected());
        return ToSelf();
      }

      var data = Get<CuttingCounterLogic.Data>();
      data.CurrentType = input.PlayerKitchenObjectType;
      Output(new Output.PlaceRequested());
      return To<Occupied>();
    }
  }

  [Meta, Id("cutting_counter_logic_state_occupied")]
  public partial record Occupied : CuttingCounterLogicState,
    IGet<Input.InteractAlternate>,
    IGet<Input.Interact>
  {
    public Type On(in Input.InteractAlternate input)
    {
      if (input.PlayerHasKitchenObject)
      {
        Output(new Output.InteractionRejected());
        return ToSelf();
      }

      var data = Get<CuttingCounterLogic.Data>();
      if (!CuttingRecipes.TryGet(data.CurrentType, out _, out var duration))
      {
        Output(new Output.InteractionRejected());
        return ToSelf();
      }

      data.Elapsed = 0.0;
      data.Duration = duration;
      Output(new Output.CuttingStarted(data.CurrentType, duration));
      return To<Cutting>();
    }

    public Type On(in Input.Interact input)
    {
      if (input.PlayerHasKitchenObject)
      {
        Output(new Output.InteractionRejected());
        return ToSelf();
      }

      var data = Get<CuttingCounterLogic.Data>();
      if (data.CurrentType.ToString().EndsWith("Sliced", StringComparison.Ordinal))
      {
        Output(new Output.TakeRequested());
        return To<Empty>();
      }

      Output(new Output.InteractionRejected());
      return ToSelf();
    }
  }

  [Meta, Id("cutting_counter_logic_state_cutting")]
  public partial record Cutting : CuttingCounterLogicState,
    IGet<Input.PhysicsTick>
  {
    public Type On(in Input.PhysicsTick input)
    {
      var data = Get<CuttingCounterLogic.Data>();
      data.Elapsed = Math.Min(data.Elapsed + input.Delta, data.Duration);
      Output(new Output.ProgressChanged(data.Elapsed / data.Duration));

      if (data.Elapsed < data.Duration)
      {
        return ToSelf();
      }

      CuttingRecipes.TryGet(data.CurrentType, out var outputType, out _);
      data.CurrentType = outputType;
      Output(new Output.ItemCut(outputType));
      return To<Occupied>();
    }
  }
  public static class Input
  {
    public readonly record struct Interact(
      bool PlayerHasKitchenObject,
      KitchenObjectType PlayerKitchenObjectType
    );

    public readonly record struct InteractAlternate(
      bool PlayerHasKitchenObject,
      KitchenObjectType PlayerKitchenObjectType
    );
    public readonly record struct PhysicsTick(double Delta);
  }

  public static class Output
  {
    public readonly record struct PlaceRequested;
    public readonly record struct TakeRequested;
    public readonly record struct CuttingStarted(
      KitchenObjectType Type,
      double Duration
    );
    public readonly record struct ProgressChanged(double Progress);
    public readonly record struct ItemCut(KitchenObjectType Type);
    public readonly record struct InteractionRejected;
  }
}
