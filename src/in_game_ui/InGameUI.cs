namespace ChickenChaos;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;

public interface IInGameUI : IControl
{
  void SetActionLabel(string text);
}

[Meta(typeof(IAutoNode))]
public partial class InGameUI : Control, IInGameUI
{
  public override void _Notification(int what) => this.Notify(what);

  #region Dependencies

  [Dependency] public IAppRepo AppRepo => this.DependOn<IAppRepo>();
  [Dependency] public IGameRepo GameRepo => this.DependOn<IGameRepo>();

  #endregion Dependencies

  #region Nodes

  [Node] public ILabel ActionLabel { get; set; } = default!;

  #endregion Nodes

  #region State

  public IInGameUILogic InGameUILogic { get; set; } = default!;

  public LogicBlock.Binding InGameUIBinding { get; set; } = default!;

  #endregion State

  public void Setup() => InGameUILogic = new InGameUILogic();

  public void OnResolved()
  {
    InGameUILogic.Set(this);
    InGameUILogic.Set(AppRepo);
    InGameUILogic.Set(GameRepo);

    InGameUIBinding = InGameUILogic.Bind();

    InGameUIBinding
      .OnOutput((in InGameUILogicState.Output.CurrentCounterChanged output) =>
        GD.Print($"CurrentCounterChanged: {output.Counter}")
      )
      .OnOutput((in InGameUILogicState.Output.InteractableCounterChanged output) =>
        SetActionLabel(
          output.InteractableCounter != null ? "Interact" : string.Empty
        )
      );

    InGameUILogic.Start<InGameUILogicState>();
  }

  public void SetActionLabel(string text) =>
    ActionLabel.Text = text;

  public void OnExitTree()
  {
    InGameUILogic.Stop();
    InGameUIBinding.Dispose();
  }
}
