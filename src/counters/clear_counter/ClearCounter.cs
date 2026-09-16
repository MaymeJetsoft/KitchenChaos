namespace KitchenChaos;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;

public interface IClearCounter : ICounter
{
  void Clear();
}

[Meta(typeof(IAutoNode))]
public partial class ClearCounter : Counter, IClearCounter
{
  public override void _Notification(int what) => this.Notify(what);

  #region Nodes

  [Export]
  public PackedScene KitchenObjectScene { get; set; } = null!;

  [Node]
  public IMarker3D Marker3D { get; private set; } = null!;

  #endregion Nodes

  private KitchenObject? KitchenObjectInstance { get; set; }

  public void SpawnKitchenObject()
  {
    if (KitchenObjectScene == null || KitchenObjectInstance != null)
    {
      GD.PushWarning("Cannot spawn a kitchen object.");
      return;
    }

    var kitchenObject = KitchenObjectScene.InstantiateOrNull<KitchenObject>();
    if (kitchenObject == null)
    {
      GD.PushWarning("KitchenObjectScene must have a KitchenObject root script.");
      return;
    }

    KitchenObjectInstance = kitchenObject;
    AddChild(kitchenObject);
    kitchenObject.Position = Marker3D.Position;
  }

  public void Clear()
  {
    KitchenObjectInstance?.QueueFree();
    KitchenObjectInstance = null;
  }
}
