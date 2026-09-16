namespace ChickenChaos;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public abstract partial record PlayerLogicState
{
  [Meta, Id("player_logic_state_alive_grounded_idle")]
  public partial record Idle : Grounded,
    IGet<Input.StartedMovingHorizontally>,
    IGet<Input.CounterChanged>,
    IGet<Input.InteractPressed>
  {
    public Idle()
    {
      this.OnEnter(() => Output(new Output.Animations.Idle()));
    }

    public Type On(in Input.StartedMovingHorizontally input) =>
      To<Moving>();

    public Type On(in Input.CounterChanged input)
    {
      var counter = input.Counter;
      if (counter is not null)
      {
        if (counter.CanInteract())
        {
          Get<IGameRepo>().SetInteractableCounter(counter);
        }
      }
      else
      {
        Get<IGameRepo>().SetInteractableCounter(null);
      }

      return ToSelf();
    }

    public Type On(in Input.InteractPressed input)
    {
      var counter = input.Counter;
      if (counter is not null)
      {
        if (counter.CanInteract())
        {
          Get<IGameRepo>().SetCurrentCounter(counter);
        }
      }

      return ToSelf();
    }
  }
}
