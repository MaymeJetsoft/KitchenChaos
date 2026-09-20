namespace KitchenChaos;

using Chickensoft.AutoInject;
using Chickensoft.Introspection;

public interface ICheeseBlock : IKitchenObject
{

}

[Meta(typeof(IAutoNode))]
public partial class CheeseBlock : KitchenObject, ICheeseBlock
{
  public override void _Notification(int what) => this.Notify(what);
}
