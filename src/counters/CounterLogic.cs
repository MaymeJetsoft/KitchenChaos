namespace KitchenChaos;

using ChickenChaos;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Chickensoft.Sync.Primitives;

public interface ICounterLogic : ILogicBlock;

[Meta]
public partial class CounterLogic : LogicBlock, ICounterLogic
{
  private AutoValue<ICounter?>.Binding? _interactableCounterBinding;
  private AutoValue<ICounter>.Binding? _currentCounterBinding;

  public CounterLogic()
  {
    Set(new CounterLogicState());
  }

  public override void OnStart()
  {
    var gameRepo = Get<IGameRepo>();
    _interactableCounterBinding = gameRepo.InteractableCounter.Bind()
      .OnValue((interactableCounter) => State?.Output(new CounterLogicState.Output.InteractableCounterChanged(interactableCounter)));
    _currentCounterBinding = gameRepo.CurrentCounter.Bind()
      .OnValue((currentCounter) => State?.Output(new CounterLogicState.Output.CurrentCounterChanged(currentCounter)));
  }

  public override void OnStop()
  {
    _interactableCounterBinding?.Dispose();
    _currentCounterBinding?.Dispose();
  }
}
