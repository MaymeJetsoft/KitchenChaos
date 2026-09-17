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
  void ShowInteractable();
  void HideInteractable();
  virtual void SpawnKitchenObject() => GD.PushWarning("SpawnKitchenObject is not implemented for this counter.");
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
  public void Drop() => _bearable.Drop();
  public bool HasKitchenObject() => _bearable.HasKitchenObject();
  public KitchenObject? GetKitchenObject() => _bearable.GetKitchenObject();

  #endregion Bearable

  #region State

  public ICounterLogic CounterLogic { get; set; } = default!;

  public LogicBlock.Binding CounterBinding { get; set; } = default!;

  #endregion State

  public void Setup()
  {
    AddChild(_bearable);
    CounterLogic = new CounterLogic();
  }

  public void OnResolved()
  {
    CounterLogic.Set(this);
    CounterLogic.Set(GameRepo);

    CounterBinding = CounterLogic.Bind();

    CounterBinding
      .OnOutput((in CounterLogicState.Output.CounterInteracted output) =>
      {
        if (output.Counter == this)
        {
          Interact(null!);
        }
      })
      .OnOutput((in CounterLogicState.Output.FacingCounterChanged output) =>
      {
        if (output.Counter == this)
        {
          ShowInteractable();
        }
        else
        {
          HideInteractable();
        }
      });

    CounterLogic.Start<CounterLogicState>();
  }

  public void OnExitTree()
  {
    CounterLogic.Stop();
    CounterBinding.Dispose();
  }

  public bool CanInteract() => IsInteractable;
  public void ShowInteractable()
  {
    // Highlight the counter to indicate that it can be interacted with.
    GD.Print($"Counter {Name} is now interactable. (counter)");
    AnimationPlayer.Play("highlight");

    // Show interaction icon
  }

  public void HideInteractable()
  {
    GD.Print($"Counter {Name} is no longer interactable. (counter)");
    // Remove highlight from the counter.
    AnimationPlayer.Play("RESET");

    // Hide interaction icon
  }

  public void Interact(IPlayer player)
  {
    if (!CanInteract())
    {
      GD.Print($"Counter {Name} cannot be interacted with. (counter)");
      return;
    }

    if (this is IClearCounter clearCounter)
    {
      clearCounter.SpawnKitchenObject();
    }
  }
}
