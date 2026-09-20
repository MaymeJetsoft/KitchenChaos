namespace KitchenChaos;

using Chickensoft.AutoInject;
using Chickensoft.Introspection;

public interface IMeatPattyUncooked : IKitchenObject
{

}

[Meta(typeof(IAutoNode))]
public partial class MeatPattyUncooked : KitchenObject, ICabbage
{
  public override void _Notification(int what) => this.Notify(what);
}
