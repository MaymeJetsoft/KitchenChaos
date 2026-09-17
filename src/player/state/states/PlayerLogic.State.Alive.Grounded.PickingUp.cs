namespace KitchenChaos;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public abstract partial record PlayerLogicState
{
  [Meta, Id("player_logic_state_alive_grounded_picking_up")]
  public partial record PickingUp : Grounded,
    IGet<Input.PickUpCompleted>
  {
    public PickingUp()
    {
      this.OnEnter(() =>
      {
        var kitchenObject = Get<PlayerLogic.Data>().PickupKitchenObject;
        if (kitchenObject is not null)
        {
          Output(new Output.KitchenObjectChanged(kitchenObject));
          Get<PlayerLogic.Data>().PickupKitchenObject = null;
        }
      });
    }

    public Type On(in Input.PickUpCompleted input) =>
      To<Idle>();
  }
}
