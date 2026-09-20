namespace KitchenChaos;

using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using Godot;

[Meta(typeof(IAutoNode))]
public partial class ContainerCounter : Counter
{
  [Export]
  public KitchenObjectType GeneratedType { get; set; } = KitchenObjectType.Tomato;

  [Export]
  public PackedScene KitchenObjectScene { get; set; } = default!;

  public IContainerCounterLogic ContainerCounterLogic { get; set; } = default!;
  private IPlayer? _interactingPlayer;

  public override void Setup()
  {
    base.Setup();
    ContainerCounterLogic = new ContainerCounterLogic(GeneratedType);
    CounterLogic = ContainerCounterLogic;
  }

  protected override void StartCounterLogic() =>
    CounterLogic.Start<ContainerCounterLogicState>();

  public override void OnResolved()
  {
    base.OnResolved();
    CounterBinding.OnOutput(
      (in ContainerCounterLogicState.Output.SpawnRequested output) =>
      {
        if (_interactingPlayer is null || KitchenObjectScene is null)
        {
          return;
        }

        var kitchenObject = KitchenObjectScene.Instantiate<KitchenObject>();
        kitchenObject.Type = output.Type;
        _interactingPlayer.Carry(kitchenObject);
      }
    );
  }

  public override void Interact(IPlayer player)
  {
    _interactingPlayer = player;
    ContainerCounterLogic.Input(
      new ContainerCounterLogicState.Input.Interact(player.HasKitchenObject())
    );
  }
}