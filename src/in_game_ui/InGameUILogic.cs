namespace KitchenChaos;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Chickensoft.Sync.Primitives;

public interface IInGameUILogic : ILogicBlock;

// This state machine is nothing more than glue to the game repository.
// If the UI were more sophisticated, it'd be easy to expand on this.

[Meta]
public partial class InGameUILogic : LogicBlock, IInGameUILogic
{
  private AutoValue<ICounter?>.Binding? _facingCounterBinding;

  public InGameUILogic()
  {
    Set(new InGameUILogicState());
  }

  public override void OnStart()
  {
    var gameRepo = Get<IGameRepo>();
    _facingCounterBinding = gameRepo.FacingCounter.Bind()
      .OnValue((counter) => State?.Output(new InGameUILogicState.Output.FacingCounterChanged(counter)));
  }

  public override void OnStop() => _facingCounterBinding?.Dispose();
}
