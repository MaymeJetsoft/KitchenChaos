namespace KitchenChaos;

using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using Godot;

[Meta(typeof(IAutoNode))]
public partial class TrashCounter : Counter
{
  public ITrashCounterLogic TrashCounterLogic { get; set; } = default!;
  private IPlayer? _interactingPlayer;

  public override void Setup()
  {
    base.Setup();
    TrashCounterLogic = new TrashCounterLogic();
    CounterLogic = TrashCounterLogic;
  }

  protected override void StartCounterLogic() =>
    CounterLogic.Start<TrashCounterLogicState.Empty>();

  protected override void BindCounterOutputs()
  {
    base.BindCounterOutputs();

    CounterBinding
      .OnOutput((in TrashCounterLogicState.Output.PlaceRequested _) =>
      {
        var kitchenObject = _interactingPlayer?.Take();
        if (kitchenObject is not null)
        {
          Carry(kitchenObject);

          if (AnimationPlayer is not null)
          {
            AnimationPlayer.Play("destroying");
            ToSignal(AnimationPlayer as AnimationMixer, AnimationMixer.SignalName.AnimationFinished).GetResult();
          }

          Drop();
        }
      });
  }

  public override void Interact(IPlayer player)
  {
    _interactingPlayer = player;
    TrashCounterLogic.Input(new TrashCounterLogicState.Input.Interact(
      player.HasKitchenObject()
    ));
  }
}
