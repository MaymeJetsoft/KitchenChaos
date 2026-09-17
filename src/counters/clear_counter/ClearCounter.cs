namespace KitchenChaos;

using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using Godot;

public interface IClearCounter : ICounter
{
}

[Meta(typeof(IAutoNode))]
public partial class ClearCounter : Counter, IClearCounter
{
  public override void _Notification(int what) => this.Notify(what);

  #region Nodes

  [Export]
  public PackedScene KitchenObjectScene { get; set; } = null!;

  #endregion Nodes

  public void SpawnKitchenObject()
  {
    if (KitchenObjectScene == null || GetKitchenObject() != null)
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

    Carry(kitchenObject);
  }
}
