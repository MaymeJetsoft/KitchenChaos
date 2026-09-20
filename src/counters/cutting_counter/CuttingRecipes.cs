namespace KitchenChaos;

public static class CuttingRecipes
{
  public const double TomatoCuttingDuration = 3.0;

  public static bool TryGet(
    KitchenObjectType input,
    out KitchenObjectType output,
    out double duration
  )
  {
    if (input == KitchenObjectType.Tomato)
    {
      output = KitchenObjectType.SlicedTomato;
      duration = TomatoCuttingDuration;
      return true;
    }

    output = KitchenObjectType.None;
    duration = 0.0;
    return false;
  }
}
