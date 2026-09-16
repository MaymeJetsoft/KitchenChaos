namespace KitchenChaos;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;

public interface ITomato : IKitchenObject
{

}

[Meta(typeof(IAutoNode))]
public partial class Tomato : KitchenObject, ITomato
{
  public override void _Notification(int what) => this.Notify(what);
}
