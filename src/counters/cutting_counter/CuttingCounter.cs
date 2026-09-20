namespace KitchenChaos;

using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using Godot;

[Meta(typeof(IAutoNode))]
public partial class CuttingCounter : Counter
{
  public ICuttingCounterLogic CuttingCounterLogic { get; set; } = default!;
  private IPlayer? _interactingPlayer;

  public override void Setup()
  {
    base.Setup();
    CuttingCounterLogic = new CuttingCounterLogic();
    CounterLogic = CuttingCounterLogic;
  }

  protected override void StartCounterLogic() =>
    CounterLogic.Start<CuttingCounterLogicState.Empty>();

  protected override void BindCounterOutputs()
  {
    base.BindCounterOutputs();

    CounterBinding
      .OnOutput((in CuttingCounterLogicState.Output.PlaceRequested _) =>
      {
        var kitchenObject = _interactingPlayer?.Take();
        if (kitchenObject is not null)
        {
          Carry(kitchenObject);
        }
      })
      .OnOutput((in CuttingCounterLogicState.Output.TakeRequested _) =>
      {
        var kitchenObject = Take();
        if (kitchenObject is not null)
        {
          _interactingPlayer?.Carry(kitchenObject);
        }
      })
      .OnOutput((in CuttingCounterLogicState.Output.ItemCut output) =>
      {
        var previousObject = Take();
        var slicedScene = previousObject?.KitchenObjectSlicedScene;
        previousObject?.QueueFree();

        if (slicedScene is null)
        {
          return;
        }

        var slicedObject = slicedScene.Instantiate<KitchenObject>();
        Carry(slicedObject);
      });
  }

  public override void _PhysicsProcess(double delta)
  {
    CuttingCounterLogic.Input(
      new CuttingCounterLogicState.Input.PhysicsTick(delta)
    );
  }

  public override void Interact(IPlayer player)
  {
    _interactingPlayer = player;
    CuttingCounterLogic.Input(new CuttingCounterLogicState.Input.Interact(
      player.HasKitchenObject(),
      player.GetKitchenObject()?.Type ?? KitchenObjectType.None
    ));
  }

  public override void InteractAlternate(IPlayer player)
  {
    _interactingPlayer = player;
    CuttingCounterLogic.Input(new CuttingCounterLogicState.Input.InteractAlternate(
      player.HasKitchenObject(),
      player.GetKitchenObject()?.Type ?? KitchenObjectType.None
    ));
  }
}
