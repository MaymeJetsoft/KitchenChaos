namespace KitchenChaos;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;

public interface ICounter : IStaticBody3D, IBearable
{
  bool IsPlayerMovementBlocked { get; }

  bool CanInteract();
  bool CanInteractAlternate();
  void Interact(IPlayer player);
  void InteractAlternate(IPlayer player);
  void ShowInteractable();
  void HideInteractable();
}

[Meta(typeof(IAutoNode))]
public partial class Counter : StaticBody3D, ICounter
{
  public override void _Notification(int what) => this.Notify(what);

  #region Properties

  [Export]
  public bool IsInteractable { get; set; } = true;

  [Export]
  public bool IsInteractableAlternate { get; set; } = false;

  [Export]
  public virtual bool IsPlayerMovementBlocked { get; set; } = false;

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
  public bool CanInteractAlternate() => IsInteractableAlternate;
  public bool HasKitchenObject() => _bearable.HasKitchenObject();
  public KitchenObject? GetKitchenObject() => _bearable.GetKitchenObject();

  public virtual void Interact(IPlayer player) =>
    GD.PushWarning($"{GetType().Name} does not implement interaction.");

  public virtual void InteractAlternate(IPlayer player) =>
    GD.PushWarning($"{GetType().Name} does not implement alternate interaction.");

  public void ShowInteractable() => AnimationPlayer.Play("highlight");
  public void HideInteractable() => AnimationPlayer.Play("RESET");

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

  protected virtual void BindCounterOutputs() =>
    CounterBinding.OnOutput(
      (in CounterLogicState.Output.FacingCounterChanged output) =>
      {
        if (output.Counter == this)
        {
          ShowInteractable();
        }
        else
        {
          HideInteractable();
        }
      }
    );

  public virtual void OnResolved()
  {
    CounterLogic.Set(this as ICounter);
    CounterLogic.Set(GameRepo);

    CounterBinding = CounterLogic.Bind();

    BindCounterOutputs();

    StartCounterLogic();
  }

  public void OnExitTree()
  {
    CounterLogic.Stop();
    CounterBinding.Dispose();
  }
}
