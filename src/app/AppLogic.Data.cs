namespace KitchenChaos;

public partial class AppLogic
{
  public record Data
  {
    public bool ShouldLoadExistingGame { get; set; }
    public PostGameAction PostGameAction { get; set; }
  }
}
