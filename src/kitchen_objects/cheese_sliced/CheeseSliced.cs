namespace KitchenChaos;

using Chickensoft.AutoInject;
using Chickensoft.Introspection;

public interface ICheeseSliced : IKitchenObject
{

}

[Meta(typeof(IAutoNode))]
public partial class CheeseSliced : KitchenObject, ICheeseBlock
{
  public override void _Notification(int what) => this.Notify(what);
}
