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

  public CounterLogic()
  {
    Set(new CounterLogicState());
  }

  public override void OnStart()
  {
    var gameRepo = Get<IGameRepo>();
    _interactableCounterBinding = gameRepo.InteractableCounter.Bind()
      .OnValue((interactableCounter) => State?.Output(new CounterLogicState.Output.InteractableCounterChanged(interactableCounter)));
  }

  public override void OnStop() => _interactableCounterBinding?.Dispose();
}
