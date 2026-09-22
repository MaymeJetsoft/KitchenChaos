namespace KitchenChaos;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;

[Meta(typeof(IAutoNode))]
public partial class CuttingCounter : Counter
{
  [Export]
  public Godot.Collections.Array<KitchenObjectToSlices> KitchenObjectToSlices { get; set; } = default!;

  [Node]
  public IProgressBar ProgressBar { get; set; } = default!;

  public ICuttingCounterLogic CuttingCounterLogic { get; set; } = default!;
  private IPlayer? _interactingPlayer;

  public override void Setup()
  {
    base.Setup();
    CuttingCounterLogic = new CuttingCounterLogic();
    CounterLogic = CuttingCounterLogic;
  }

  protected override void StartCounterLogic()
  {
    CounterLogic.Start<CuttingCounterLogicState.Empty>();
    ResetProgressBar();
  }

  protected override void BindCounterOutputs()
  {
    base.BindCounterOutputs();

    CounterBinding
      .OnOutput((in CuttingCounterLogicState.Output.PlaceRequested _) =>
      {
        var kitchenObject = _interactingPlayer?.Take();
        if (kitchenObject is not null)
        {
          Carry(kitchenObject);
        }
      })
      .OnOutput((in CuttingCounterLogicState.Output.TakeRequested _) =>
      {
        var kitchenObject = Take();
        if (kitchenObject is not null)
        {
          _interactingPlayer?.Carry(kitchenObject);
        }
      })
      .OnOutput((in CuttingCounterLogicState.Output.CuttingStarted output) =>
      {
        ResetProgressBar();
        ProgressBar.Visible = true;
      })
      .OnOutput((in CuttingCounterLogicState.Output.CuttingProgressed output) => ProgressBar.Value = output.Value)
      .OnOutput((in CuttingCounterLogicState.Output.ItemCut output) =>
      {
        ResetProgressBar();

        var previousObject = Take();
        var slicedScene = previousObject?.KitchenObjectSlicedScene;
        previousObject?.QueueFree();

        if (slicedScene is null)
        {
          return;
        }

        var slicedObject = slicedScene.Instantiate<KitchenObject>();
        Carry(slicedObject);
      });
  }

  public override void _PhysicsProcess(double delta)
  {
    CuttingCounterLogic.Input(
      new CuttingCounterLogicState.Input.PhysicsTick(delta)
    );
  }

  public override void Interact(IPlayer player)
  {
    _interactingPlayer = player;
    CuttingCounterLogic.Input(new CuttingCounterLogicState.Input.Interact(
      player.HasKitchenObject(),
      player.GetKitchenObject()?.Type ?? KitchenObjectType.None
    ));
  }

  public override void InteractAlternate(IPlayer player)
  {
    _interactingPlayer = player;
    CuttingCounterLogic.Input(new CuttingCounterLogicState.Input.InteractAlternate(
      player.HasKitchenObject(),
      player.GetKitchenObject()?.Type ?? KitchenObjectType.None
    ));
  }

  public bool TryGetRecipe(
      KitchenObjectType inputType,
      out KitchenObjectType outputType,
      out double duration
    )
  {
    foreach (var recipe in KitchenObjectToSlices)
    {
      if (recipe is null)
      {
        continue;
      }

      var inputObject = recipe.Input?.Instantiate<KitchenObject>();
      if (inputObject is null)
      {
        continue;
      }

      var isMatch = inputObject.Type == inputType;
      inputObject.QueueFree();

      if (!isMatch)
      {
        continue;
      }

      var outputObject = recipe.Output?.Instantiate<KitchenObject>();
      if (outputObject is null)
      {
        outputType = KitchenObjectType.None;
        duration = 0.0;
        return false;
      }

      outputType = outputObject.Type;
      duration = recipe.Duration;
      outputObject.QueueFree();
      return true;
    }

    outputType = KitchenObjectType.None;
    duration = 0.0;
    return false;
  }

  public void ResetProgressBar()
  {
    ProgressBar.Visible = false;
    ProgressBar.Value = 0;
  }
}
