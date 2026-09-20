namespace KitchenChaos;

using Chickensoft.AutoInject;
using Chickensoft.Introspection;

[Meta(typeof(IAutoNode))]
public partial class ClearCounter : Counter
{
  public IClearCounterLogic ClearCounterLogic { get; set; } = default!;
  private IPlayer? _interactingPlayer;

  public override void Setup()
  {
    base.Setup();
    ClearCounterLogic = new ClearCounterLogic();
    CounterLogic = ClearCounterLogic;
  }

  protected override void StartCounterLogic() =>
    CounterLogic.Start<ClearCounterLogicState>();

  protected override void BindCounterOutputs()
  {
    base.BindCounterOutputs();

    CounterBinding
      .OnOutput((in ClearCounterLogicState.Output.PlaceRequested _) =>
      {
        var kitchenObject = _interactingPlayer?.Take();
        if (kitchenObject is not null)
        {
          Carry(kitchenObject);
        }
      })
      .OnOutput((in ClearCounterLogicState.Output.TakeRequested _) =>
      {
        var kitchenObject = Take();
        if (kitchenObject is not null)
        {
          _interactingPlayer?.Carry(kitchenObject);
        }
      });
  }

  public override void Interact(IPlayer player)
  {
    _interactingPlayer = player;
    ClearCounterLogic.Input(new ClearCounterLogicState.Input.Interact(
      player.HasKitchenObject(),
      HasKitchenObject()
    ));
  }
}
