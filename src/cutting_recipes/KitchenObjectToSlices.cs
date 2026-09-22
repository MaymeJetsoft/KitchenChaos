namespace KitchenChaos;

using Godot;

[GlobalClass]
public partial class KitchenObjectToSlices : Resource
{
  [Export]
  public PackedScene Input { get; set; } = default!;

  [Export]
  public PackedScene Output { get; set; } = default!;

  [Export]
  public double Duration { get; set; } = 3.0;
}
