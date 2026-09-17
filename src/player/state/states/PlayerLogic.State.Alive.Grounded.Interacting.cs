namespace KitchenChaos;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;

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
        FinishInteraction();
      });
    }

    public Type On(in Input.InteractionCompleted input) =>
      To<Idle>();

    private void FinishInteraction()
    {
      //TODO: need to trigger interactionCompleted from OnAnimationFinished
      var counter = Get<PlayerLogic.Data>().CurrentCounter;
      Get<PlayerLogic.Data>().CurrentCounter = null;
      GD.Print("Interaction completed with Counter: " + counter);
      Input(new Input.InteractionCompleted());
    }
  }
}
