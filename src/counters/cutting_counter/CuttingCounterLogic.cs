namespace KitchenChaos;

using Chickensoft.Introspection;

public interface ICuttingCounterLogic : ICounterLogic;

[Meta]
public partial class CuttingCounterLogic : CounterLogic, ICuttingCounterLogic
{
  [Meta]
  public partial class Data
  {
    public KitchenObjectType CurrentType { get; set; }
    public double Elapsed { get; set; }
    public double Duration { get; set; }
  }

  public CuttingCounterLogic()
  {
    Set(new Data());
    Set(new CuttingCounterLogicState.Empty());
    Set(new CuttingCounterLogicState.Occupied());
    Set(new CuttingCounterLogicState.Cutting());
  }
}
