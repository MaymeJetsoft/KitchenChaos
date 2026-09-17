namespace KitchenChaos;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;

public abstract partial record PlayerLogicState
{
  [Meta, Id("player_logic_state_alive_grounded_idle")]
  public partial record Idle : Grounded,
    IGet<Input.StartedMovingHorizontally>,
    IGet<Input.FacingCounterChanged>,
    IGet<Input.InteractionStarted>
  {
    public Idle()
    {
      this.OnEnter(() => Output(new Output.Animations.Idle()));
    }

    public Type On(in Input.StartedMovingHorizontally input) =>
      To<Moving>();

    public Type On(in Input.FacingCounterChanged input)
    {
      var counter = input.Counter;
      if (counter is not null)
      {
        if (counter.CanInteract())
        {
          Get<IGameRepo>().SetFacingCounter(counter);
        }
      }
      else
      {
        Get<IGameRepo>().SetFacingCounter(null);
      }

      return ToSelf();
    }

    public Type On(in Input.InteractionStarted input)
    {
      var counter = input.Counter;
      if (counter is not null && counter.CanInteract())
      {
        if (counter is IBearable bearable && bearable.HasKitchenObject())
        {
          Get<PlayerLogic.Data>().PickupKitchenObject = bearable.GetKitchenObject();
          GD.Print("Pickup KitchenObject: " + Get<PlayerLogic.Data>().PickupKitchenObject);
          return To<PickingUp>();
        }
        else
        {
          Get<PlayerLogic.Data>().CurrentCounter = counter;
          GD.Print("Interacting with Current Counter: " + Get<PlayerLogic.Data>().CurrentCounter);
          return To<Interacting>();
        }
      }

      return ToSelf();
    }
  }
}
