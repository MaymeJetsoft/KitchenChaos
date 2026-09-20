namespace KitchenChaos;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public abstract partial record PlayerLogicState
{
  [Meta, Id("player_logic_state_alive_grounded_idle")]
  public partial record Idle : Grounded,
    IGet<Input.StartedMovingHorizontally>,
    IGet<Input.InteractionStarted>,
    IGet<Input.InteractionAlternateStarted>
  {
    public Idle()
    {
      this.OnEnter(() => Output(new Output.Animations.Idle()));
    }

    public Type On(in Input.StartedMovingHorizontally input) =>
      To<Moving>();

    public Type On(in Input.InteractionStarted input)
    {
      if (input.Counter.CanInteract())
      {
        Get<PlayerLogic.Data>().CurrentCounter = input.Counter;
        return To<Interacting>();
      }

      return ToSelf();
    }

    public Type On(in Input.InteractionAlternateStarted input)
    {
      if (input.Counter.CanInteractAlternate())
      {
        Get<PlayerLogic.Data>().CurrentCounter = input.Counter;
        return To<InteractingAlternate>();
      }

      return ToSelf();
    }
  }
}
