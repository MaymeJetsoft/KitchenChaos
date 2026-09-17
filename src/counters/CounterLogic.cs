namespace KitchenChaos;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Chickensoft.Sync.Primitives;

public interface ICounterLogic : ILogicBlock;

[Meta]
public partial class CounterLogic : LogicBlock, ICounterLogic
{
  private AutoValue<ICounter?>.Binding? _facingCounterBinding;

  public CounterLogic()
  {
    Set(new CounterLogicState());
  }

  public override void OnStart()
  {
    var gameRepo = Get<IGameRepo>();
    _facingCounterBinding = gameRepo.FacingCounter.Bind()
      .OnValue((counter) => State?.Output(new CounterLogicState.Output.FacingCounterChanged(counter)));
  }

  public override void OnStop() => _facingCounterBinding?.Dispose();
}
