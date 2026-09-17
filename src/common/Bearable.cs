namespace KitchenChaos;

using System;
using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;

public interface IBearable : INode3D
{
  void Carry(KitchenObject carryingObject);
  void Drop();
  bool HasKitchenObject();
  KitchenObject? GetKitchenObject();

  IMarker3D CarryingPosition { get; set; }
}

[Meta(typeof(IAutoNode))]
public sealed partial class Bearable : Node3D, IBearable
{
  public override void _Notification(int what) => this.Notify(what);

  private KitchenObject? CarryingObject { get; set; }
  public IMarker3D CarryingPosition { get; set; } = null!;

  public void Carry(KitchenObject carryingObject)
  {
    CarryingObject = carryingObject;
    if (CarryingPosition != null)
    {
      var parent = carryingObject.GetParent();
      if (parent is Bearable bearable)
      {
        bearable.CarryingObject = null;
      }
      parent?.RemoveChild(carryingObject);
      CarryingPosition.AddChild(carryingObject);
    }
  }

  public void Drop()
  {
    CarryingObject?.QueueFree();
    CarryingObject = null;
  }

  public bool HasKitchenObject() => CarryingObject != null;
  public KitchenObject? GetKitchenObject() => CarryingObject;
}
