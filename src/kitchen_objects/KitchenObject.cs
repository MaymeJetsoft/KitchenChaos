namespace KitchenChaos;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;

public interface IKitchenObject : IStaticBody3D
{

}

[Meta(typeof(IAutoNode))]
public partial class KitchenObject : StaticBody3D, IKitchenObject
{
  public override void _Notification(int what) => this.Notify(what);

}
