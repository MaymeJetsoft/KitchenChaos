namespace KitchenChaos;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public abstract partial record PlayerLogicState
{
  [Meta, Id("player_logic_state_alive_grounded_interacting")]
  public partial record Interacting : Grounded,
    IGet<Input.InteractionCompleted>
  {
    public Interacting()
    {
      this.OnEnter(() =>
      {
        var counter = Get<PlayerLogic.Data>().CurrentCounter;
        counter?.Interact(Get<IPlayer>());
        Input(new Input.InteractionCompleted(counter));
      });
    }

    public Type On(in Input.InteractionCompleted input)
    {
      Get<PlayerLogic.Data>().CurrentCounter = null;
      return To<Idle>();
    }
  }
}
