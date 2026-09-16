namespace KitchenChaos;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;

public interface IClearCounter
{
  void Clear();
}

[Meta(typeof(IAutoNode))]
public partial class ClearCounter : Counter, IClearCounter
{
  public override void _Notification(int what) => this.Notify(what);


  public void Clear() => throw new System.NotImplementedException();
}
