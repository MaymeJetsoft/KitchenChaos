namespace KitchenChaos;

using Chickensoft.Introspection;

public interface IClearCounterLogic : ICounterLogic;

[Meta]
public partial class ClearCounterLogic : CounterLogic, IClearCounterLogic
{
  public ClearCounterLogic()
  {
    Set(new ClearCounterLogicState());
  }
}
