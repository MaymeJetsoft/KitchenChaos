namespace KitchenChaos;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public abstract partial record PlayerLogicState
{
  [Meta, Id("player_logic_state_alive_grounded_interacting_alternate")]
  public partial record InteractingAlternate : Grounded,
    IGet<Input.InteractionAlternateCompleted>
  {
    public InteractingAlternate()
    {
      this.OnEnter(() =>
      {
        var counter = Get<PlayerLogic.Data>().CurrentCounter;
        counter?.InteractAlternate(Get<IPlayer>());
        Input(new Input.InteractionAlternateCompleted(counter));
      });
    }

    public Type On(in Input.InteractionAlternateCompleted input)
    {
      Get<PlayerLogic.Data>().CurrentCounter = null;
      return To<Idle>();
    }
  }
}
