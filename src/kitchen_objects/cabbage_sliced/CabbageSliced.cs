namespace KitchenChaos;

using Chickensoft.AutoInject;
using Chickensoft.Introspection;

public interface ICabbageSliced : IKitchenObject
{

}

[Meta(typeof(IAutoNode))]
public partial class CabbageSliced : KitchenObject, ITomato
{
  public override void _Notification(int what) => this.Notify(what);
}
