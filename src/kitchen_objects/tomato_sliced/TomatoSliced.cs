namespace KitchenChaos;

using Chickensoft.AutoInject;
using Chickensoft.Introspection;

public interface ITomatoSliced : IKitchenObject
{

}

[Meta(typeof(IAutoNode))]
public partial class TomatoSliced : KitchenObject, ITomato
{
  public override void _Notification(int what) => this.Notify(what);
}
