namespace KitchenChaos;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public interface IClearCounterLogic : ICounterLogic;

[Meta]
public partial class ClearCounterLogic : LogicBlock, IClearCounterLogic
{
  public ClearCounterLogic()
  {
    Set(new ClearCounterLogicState());
  }
}