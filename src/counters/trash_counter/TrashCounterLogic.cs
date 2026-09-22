namespace KitchenChaos;

using Chickensoft.Introspection;

public interface ITrashCounterLogic : ICounterLogic;

[Meta]
public partial class TrashCounterLogic : CounterLogic, ITrashCounterLogic
{
  public TrashCounterLogic()
  {
    Set(new TrashCounterLogicState.Empty());
  }
}
