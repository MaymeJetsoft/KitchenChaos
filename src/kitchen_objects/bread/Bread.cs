namespace KitchenChaos;

using Chickensoft.AutoInject;
using Chickensoft.Introspection;

public interface IBread : IKitchenObject
{

}

[Meta(typeof(IAutoNode))]
public partial class Bread : KitchenObject, IBread
{
  public override void _Notification(int what) => this.Notify(what);
}
