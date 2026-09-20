namespace KitchenChaos;

using System;
using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;

public interface IBearable : INode3D
{
  void Carry(KitchenObject carryingObject);
  KitchenObject? Take();
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
    if (CarryingObject is not null && CarryingObject != carryingObject)
    {
      throw new InvalidOperationException("A bearable can only carry one kitchen object.");
    }

    carryingObject.Carrier?.Take();
    carryingObject.Carrier = this;

    CarryingObject = carryingObject;
    if (CarryingPosition != null)
    {
      var parent = carryingObject.GetParent();
      parent?.RemoveChild(carryingObject);
      CarryingPosition.AddChild(carryingObject);
    }
  }

  public KitchenObject? Take()
  {
    var carryingObject = CarryingObject;
    CarryingObject = null;
    if (carryingObject is not null)
    {
      carryingObject.Carrier = null;
    }
    carryingObject?.GetParent()?.RemoveChild(carryingObject);
    return carryingObject;
  }

  public void Drop()
  {
    if (CarryingObject is not null)
    {
      CarryingObject.Carrier = null;
    }
    CarryingObject?.QueueFree();
    CarryingObject = null;
  }

  public bool HasKitchenObject() => CarryingObject != null;
  public KitchenObject? GetKitchenObject() => CarryingObject;
}
