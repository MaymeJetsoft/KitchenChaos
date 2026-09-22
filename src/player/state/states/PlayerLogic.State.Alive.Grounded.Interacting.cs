namespace KitchenChaos;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public abstract partial record PlayerLogicState
{
  [Meta, Id("player_logic_state_alive_grounded_interacting")]
  public partial record Interacting : Grounded,
    IGet<Input.InteractionCompleted>,
    IGet<Input.PhysicsTick>
  {
    public Interacting()
    {
      this.OnEnter(() =>
      {
        var counter = Get<PlayerLogic.Data>().CurrentCounter;
        counter?.Interact(Get<IPlayer>());
        if (counter is null || !counter.IsPlayerMovementBlocked)
        {
          Input(new Input.InteractionCompleted(counter));
        }
      });
    }

    public override Type On(in Input.PhysicsTick input)
    {
      var counter = Get<PlayerLogic.Data>().CurrentCounter;
      if (counter?.IsPlayerMovementBlocked == true)
      {
        var velocity = Get<IPlayer>().Velocity;
        Output(new Output.VelocityChanged(velocity with { X = 0f, Z = 0f }));
        return ToSelf();
      }

      Get<PlayerLogic.Data>().CurrentCounter = null;
      return To<Idle>();
    }

    public Type On(in Input.InteractionCompleted input)
    {
      Get<PlayerLogic.Data>().CurrentCounter = null;
      return To<Idle>();
    }
  }
}
