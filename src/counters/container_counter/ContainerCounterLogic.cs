namespace KitchenChaos;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public interface IContainerCounterLogic : ICounterLogic;

[Meta]
public partial class ContainerCounterLogic : CounterLogic, IContainerCounterLogic
{
  [Meta]
  public partial class Data
  {
    public KitchenObjectType Type { get; set; } = KitchenObjectType.None;

    public Data() { }

    public Data(KitchenObjectType type)
    {
      Type = type;
    }
  }

  public ContainerCounterLogic() : this(KitchenObjectType.None) { }

  public ContainerCounterLogic(KitchenObjectType type)
  {
    Set(new Data(type));
    Set(new ContainerCounterLogicState());
  }
}
