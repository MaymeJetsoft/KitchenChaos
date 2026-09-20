namespace KitchenChaos;

using System;
using Chickensoft.Sync.Primitives;
using Godot;

public interface IGameRepo : IDisposable
{
  IAutoChannel AutoChannel { get; }
  /// <summary>Event invoked when the game ends.</summary>
  readonly record struct Ended(GameOverReason Reason);

  readonly record struct FacingCounterChanged(ICounter? Counter);

  /// <summary>Mouse captured status.</summary>
  IAutoValue<bool> IsMouseCaptured { get; }

  /// <summary>Pause status.</summary>
  IAutoValue<bool> IsPaused { get; }

  /// <summary>Player's position in global coordinates.</summary>
  IAutoValue<Vector3> PlayerGlobalPosition { get; }

  /// <summary>Camera's global transform basis.</summary>
  IAutoValue<Basis> CameraBasis { get; }

  /// <summary>Camera's global forward direction vector.</summary>
  Vector3 GlobalCameraDirection { get; }

  /// <summary>Current counter the player is facing.</summary>
  IAutoValue<ICounter?> FacingCounter { get; }

  // /// <summary>Current counter the player is interacting with.</summary>
  // IAutoValue<ICounter> PlayerJustInteracted { get; }

  /// <summary>Inform the game that the game ended.</summary>
  /// <param name="reason">Game over reason.</param>
  void OnGameEnded(GameOverReason reason);

  /// <summary>Pauses the game and releases the mouse.</summary>
  void Pause();

  /// <summary>Resumes the game and recaptures the mouse.</summary>
  void Resume();

  // /// <summary>Tells the game that the player jumped.</summary>
  // void OnJump();

  /// <summary>Changes whether the mouse is captured or not.</summary>
  /// <param name="isMouseCaptured">
  ///   Whether or not the mouse is captured.
  /// </param>
  void SetIsMouseCaptured(bool isMouseCaptured);

  /// <summary>Sets the camera's global transform basis.</summary>
  /// <param name="cameraBasis">Camera global transform basis.</param>
  void SetCameraBasis(Basis cameraBasis);

  /// <summary>Sets the player's global position.</summary>
  /// <param name="playerGlobalPosition">
  ///   Player's global position in world
  ///   coordinates.
  /// </param>
  void SetPlayerGlobalPosition(Vector3 playerGlobalPosition);

  /// <summary>
  ///  Sets the current counter the player is facing.
  /// </summary>
  /// <param name="counter"></param>
  void SetFacingCounter(ICounter? counter);
}

/// <summary>
///   Game repository — stores pure game logic that's not directly related to the
///   game node's overall view.
/// </summary>
public class GameRepo : IGameRepo
{
  private readonly AutoChannel _autoChannel = new();
  public IAutoChannel AutoChannel => _autoChannel;

  public IAutoValue<bool> IsMouseCaptured => _isMouseCaptured;
  private readonly AutoValue<bool> _isMouseCaptured;
  public IAutoValue<bool> IsPaused => _isPaused;
  private readonly AutoValue<bool> _isPaused;
  public IAutoValue<Vector3> PlayerGlobalPosition => _playerGlobalPosition;
  private readonly AutoValue<Vector3> _playerGlobalPosition;

  public IAutoValue<Basis> CameraBasis => _cameraBasis;
  private readonly AutoValue<Basis> _cameraBasis;

  public Vector3 GlobalCameraDirection => -_cameraBasis.Value.Z;

  public IAutoValue<ICounter?> FacingCounter => _facingCounter;
  private readonly AutoValue<ICounter?> _facingCounter;

  private bool _disposedValue;

  public GameRepo()
  {
    _isMouseCaptured = new AutoValue<bool>(false);
    _isPaused = new AutoValue<bool>(false);
    _playerGlobalPosition = new AutoValue<Vector3>(Vector3.Zero);
    _cameraBasis = new AutoValue<Basis>(Basis.Identity);
    _facingCounter = new AutoValue<ICounter?>(null);
  }

  internal GameRepo(
    AutoValue<bool> isMouseCaptured,
    AutoValue<bool> isPaused,
    AutoValue<Vector3> playerGlobalPosition,
    AutoValue<Basis> cameraBasis,
    AutoValue<ICounter?> facingCounter
  )
  {
    _isMouseCaptured = isMouseCaptured;
    _isPaused = isPaused;
    _playerGlobalPosition = playerGlobalPosition;
    _cameraBasis = cameraBasis;
    _facingCounter = facingCounter;
  }

  public void SetPlayerGlobalPosition(Vector3 playerGlobalPosition) =>
    _playerGlobalPosition.Value = playerGlobalPosition;

  public void SetIsMouseCaptured(bool isMouseCaptured) =>
    _isMouseCaptured.Value = isMouseCaptured;

  public void SetCameraBasis(Basis cameraBasis) =>
    _cameraBasis.Value = cameraBasis;

  public void SetFacingCounter(ICounter? counter)
  {
    _facingCounter.Value = counter;
    _autoChannel.Send(new IGameRepo.FacingCounterChanged(counter));
  }

  public void OnGameEnded(GameOverReason reason)
  {
    _isMouseCaptured.Value = false;
    Pause();
    _autoChannel.Send(new IGameRepo.Ended(reason));
  }

  public void Pause()
  {
    _isMouseCaptured.Value = false;
    _isPaused.Value = true;
  }

  public void Resume()
  {
    _isMouseCaptured.Value = true;
    _isPaused.Value = false;
  }

  // public void OnJumpshroomUsed() => _autoChannel.Send(new IGameRepo.JumpshroomUsed());

  // public void SetNumCoinsAtStart(int numCoinsAtStart) =>
  //   _numCoinsAtStart.Value = numCoinsAtStart;

  // public void SetNumCoinsCollected(int numCoinsCollected) =>
  //   _numCoinsCollected.Value = numCoinsCollected;

  #region Internals

  protected void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      if (disposing)
      {
        // Dispose managed objects.
        _isMouseCaptured.Dispose();
        _playerGlobalPosition.Dispose();
        _cameraBasis.Dispose();
        _facingCounter.Dispose();
      }

      _disposedValue = true;
    }
  }

  public void Dispose()
  {
    Dispose(disposing: true);
    GC.SuppressFinalize(this);
  }

  #endregion Internals
}
