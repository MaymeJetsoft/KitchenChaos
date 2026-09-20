namespace KitchenChaos;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;

public interface ICounter : IStaticBody3D, IBearable
{
  bool CanInteract();
  void Interact(IPlayer player);
  // void ShowInteractable();
  // void HideInteractable();
  // virtual void SpawnKitchenObject() => GD.PushWarning("SpawnKitchenObject is not implemented for this counter.");
}

[Meta(typeof(IAutoNode))]
public partial class Counter : StaticBody3D, ICounter
{
  public override void _Notification(int what) => this.Notify(what);

  #region Properties

  [Export]
  public bool IsInteractable { get; set; } = true;

  #endregion Properties

  #region Dependencies

  [Dependency] public IGameRepo GameRepo => this.DependOn<IGameRepo>();

  #endregion Dependencies

  #region Nodes

  [Node]
  public IAnimationPlayer AnimationPlayer { get; set; } = default!;

  #endregion Nodes

  #region Bearable

  private readonly Bearable _bearable = new();

  [Node]
  public IMarker3D CarryingPosition
  {
    get => _bearable.CarryingPosition;
    set => _bearable.CarryingPosition = value;
  }

  public void Carry(KitchenObject carryingObject) => _bearable.Carry(carryingObject);
  public KitchenObject? Take() => _bearable.Take();
  public void Drop() => _bearable.Drop();
  public bool CanInteract() => IsInteractable;
  public bool HasKitchenObject() => _bearable.HasKitchenObject();
  public KitchenObject? GetKitchenObject() => _bearable.GetKitchenObject();

  public virtual void Interact(IPlayer player) =>
    GD.PushWarning($"{GetType().Name} does not implement interaction.");

  #endregion Bearable

  #region State

  public ICounterLogic CounterLogic { get; set; } = default!;

  public LogicBlock.Binding CounterBinding { get; set; } = default!;

  #endregion State

  public virtual void Setup()
  {
    AddChild(_bearable);
    CounterLogic = new CounterLogic();
  }

  protected virtual void StartCounterLogic() =>
    CounterLogic.Start<CounterLogicState>();

  public virtual void OnResolved()
  {
    CounterLogic.Set(this);
    CounterLogic.Set(GameRepo);

    CounterBinding = CounterLogic.Bind();

    // CounterBinding
    // // .OnOutput((in CounterLogicState.Output.CounterInteracted output) =>
    // // {
    // //   if (output.Counter == this)
    // //   {
    // //     Interact(null!);
    // //   }
    // // })
    // // .OnOutput((in CounterLogicState.Output.FacingCounterChanged output) =>
    // // {
    // //   if (output.Counter == this)
    // //   {
    // //     ShowInteractable();
    // //   }
    // //   else
    // //   {
    // //     HideInteractable();
    // //   }
    // // })
    // ;

    StartCounterLogic();
  }

  public void OnExitTree()
  {
    CounterLogic.Stop();
    CounterBinding.Dispose();
  }
}
