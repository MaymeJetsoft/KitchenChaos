namespace KitchenChaos;

public static class CuttingRecipes
{
  public const double TOMATO_CUTTING_DURATION = 3.0;

  public static bool TryGet(
    KitchenObjectType input,
    out KitchenObjectType output,
    out double duration
  )
  {
    if (input == KitchenObjectType.Tomato)
    {
      output = KitchenObjectType.TomatoSliced;
      duration = TOMATO_CUTTING_DURATION;
      return true;
    }
    else if (input == KitchenObjectType.CheeseBlock)
    {
      output = KitchenObjectType.CheeseSliced;
      duration = TOMATO_CUTTING_DURATION;
      return true;
    }
    else if (input == KitchenObjectType.Cabbage)
    {
      output = KitchenObjectType.CabbageSliced;
      duration = TOMATO_CUTTING_DURATION;
      return true;
    }

    output = KitchenObjectType.None;
    duration = 0.0;
    return false;
  }
}
