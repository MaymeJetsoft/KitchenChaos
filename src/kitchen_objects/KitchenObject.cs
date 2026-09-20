namespace KitchenChaos;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;

public interface IKitchenObject : IStaticBody3D
{
  KitchenObjectType Type { get; }
}

public enum KitchenObjectType
{
  None,
  Bread,
  Cabbage,
  CabbageSliced,
  MeatPattyUncooked,
  MeatPattyCooked,
  MeatPattyBurned,
  CheeseBlock,
  CheeseSliced,
  Tomato,
  TomatoSliced
}

[Meta(typeof(IAutoNode))]
public partial class KitchenObject : StaticBody3D, IKitchenObject
{
  public override void _Notification(int what) => this.Notify(what);

  internal Bearable? Carrier { get; set; }

  [Export]
  public KitchenObjectType Type { get; set; } = KitchenObjectType.None;
}
