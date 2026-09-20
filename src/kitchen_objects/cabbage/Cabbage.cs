namespace KitchenChaos;

using Chickensoft.AutoInject;
using Chickensoft.Introspection;

public interface ICabbage : IKitchenObject
{

}

[Meta(typeof(IAutoNode))]
public partial class Cabbage : KitchenObject, ICabbage
{
  public override void _Notification(int what) => this.Notify(what);
}
