namespace KitchenChaos;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;

/// <summary>
///   Camera interface. This is how the camera node is exposed to its logic block,
///   allowing the logic block to read properties of the camera without having
///   to know about its implementation. This interface can easily be mocked,
///   allowing the camera logic block to be unit-tested.
/// </summary>
public interface IPlayerCamera : INode3D
{
  Basis CameraBasis { get; }

  /// <summary>Sets the current camera to the player camera.</summary>
  void UsePlayerCamera();
}

[Meta(typeof(IAutoNode))]
public partial class PlayerCamera : Node3D, IPlayerCamera
{
  public override void _Notification(int what) => this.Notify(what);

  private PhantomCamera.PhantomCamera3D _phantomCamera = default!;

  #region Dependencies
  [Dependency] public IGameRepo GameRepo => this.DependOn<IGameRepo>();
  #endregion Dependencies

  #region Exports

  [Export(PropertyHint.ResourceType, "PlayerCameraSettings")]
  public PlayerCameraSettings Settings { get; set; } = new();

  #endregion Exports

  #region Nodes

  [Node("%Camera3D")] public ICamera3D CameraNode { get; set; } = default!;

  [Node("%PhantomCamera3D")]
  public Node3D PhantomCameraNode { get; set; } = default!;

  #endregion Nodes

  #region Computed

  public Basis CameraBasis => new(Vector3.Up, CameraNode.GlobalRotation.Y);

  #endregion Computed

  public void Setup() => SetPhysicsProcess(true);

  public void OnResolved()
  {
    _phantomCamera = new PhantomCamera.PhantomCamera3D(PhantomCameraNode);
    PublishCameraBasis();
  }

  // public void OnPhysicsProcess(double delta)
  // {
  //   var xMotion = InputUtilities.GetJoyPadActionPressedMotion(
  //     "camera_left", "camera_right", JoyAxis.RightX
  //   );

  //   if (GameRepo.IsMouseCaptured.Value && xMotion is not null)
  //   {
  //     ApplyOrbit(new Vector2(xMotion.AxisValue * Settings.JoypadSensitivity * (float)delta, 0f));
  //   }

  //   var yMotion = InputUtilities.GetJoyPadActionPressedMotion(
  //     "camera_up", "camera_down", JoyAxis.RightY
  //   );

  //   if (GameRepo.IsMouseCaptured.Value && yMotion is not null)
  //   {
  //     ApplyOrbit(new Vector2(0f, yMotion.AxisValue * Settings.JoypadSensitivity * (float)delta));
  //   }

  //   PublishCameraBasis();
  // }

  // public override void _Input(InputEvent @event)
  // {
  //   if (GameRepo.IsMouseCaptured.Value && @event is InputEventMouseMotion motion)
  //   {
  //     ApplyOrbit(motion.Relative * Settings.MouseSensitivity);
  //   }
  // }

  public void UsePlayerCamera()
  {
    _phantomCamera.Priority = 10;
    PublishCameraBasis();
  }

  // private void ApplyOrbit(Vector2 motion)
  // {
  //   var rotation = PhantomCamera.PhantomCamera3DExtensions
  //     .GetThirdPersonRotationDegrees(_phantomCamera);
  //   rotation.X = Mathf.Clamp(rotation.X - motion.Y, Settings.VerticalMin, Settings.VerticalMax);
  //   rotation.Y -= motion.X;
  //   PhantomCamera.PhantomCamera3DExtensions
  //     .SetThirdPersonRotationDegrees(_phantomCamera, rotation);
  // }

  private void PublishCameraBasis() => GameRepo.SetCameraBasis(CameraBasis);
}
