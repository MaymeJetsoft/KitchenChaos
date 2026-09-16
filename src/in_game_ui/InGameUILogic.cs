namespace ChickenChaos;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Chickensoft.Sync.Primitives;
using KitchenChaos;

public interface IInGameUILogic : ILogicBlock;

// This state machine is nothing more than glue to the game repository.
// If the UI were more sophisticated, it'd be easy to expand on this.

[Meta]
public partial class InGameUILogic : LogicBlock, IInGameUILogic
{
  private AutoValue<ICounter?>.Binding? _interactableCounterBinding;
  private AutoValue<ICounter>.Binding? _currentCounterInteractionBinding;

  public InGameUILogic()
  {
    Set(new InGameUILogicState());
  }

  public override void OnStart()
  {
    var gameRepo = Get<IGameRepo>();
    _interactableCounterBinding = gameRepo.InteractableCounter.Bind()
      .OnValue((interactableCounter) => State?.Output(new InGameUILogicState.Output.InteractableCounterChanged(interactableCounter)));
    _currentCounterInteractionBinding = gameRepo.CurrentCounter.Bind()
      .OnValue((currentCounter) => State?.Output(new InGameUILogicState.Output.CurrentCounterChanged(currentCounter)));
  }

  public override void OnStop()
  {
    _interactableCounterBinding?.Dispose();
    _currentCounterInteractionBinding?.Dispose();
  }
}
