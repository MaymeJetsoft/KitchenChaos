namespace ChickenChaos;

using System;
using Chickensoft.Sync.Primitives;
using Godot;
using KitchenChaos;

public interface IGameRepo : IDisposable
{
  IAutoChannel AutoChannel { get; }
  /// <summary>Event invoked when the game ends.</summary>
  readonly record struct Ended(GameOverReason Reason);

  // /// <summary>Event invoked whenever the player jumps.</summary>
  // readonly record struct Jumped;

  readonly record struct InteractableCounterChanged(ICounter? Counter);
  readonly record struct CurrentCounterChanged(ICounter Counter);

  /// <summary>Mouse captured status.</summary>
  IAutoValue<bool> IsMouseCaptured { get; }

  /// <summary>Pause status.</summary>
  IAutoValue<bool> IsPaused { get; }

  // /// <summary>Number of coins collected by the players.</summary>
  // IAutoValue<int> NumCoinsCollected { get; }

  // /// <summary>The total number of coins the world started with.</summary>
  // IAutoValue<int> NumCoinsAtStart { get; }

  /// <summary>Player's position in global coordinates.</summary>
  IAutoValue<Vector3> PlayerGlobalPosition { get; }

  /// <summary>Camera's global transform basis.</summary>
  IAutoValue<Basis> CameraBasis { get; }

  /// <summary>Camera's global forward direction vector.</summary>
  Vector3 GlobalCameraDirection { get; }

  /// <summary>Current counter the player is interacting with.</summary>
  IAutoValue<ICounter?> InteractableCounter { get; }

  /// <summary>Current counter the player is interacting with.</summary>
  IAutoValue<ICounter> CurrentCounter { get; }

  // /// <summary>Inform the game that the player is collecting a coin.</summary>
  // /// <param name="coin">Coin that is being collected.</param>
  // void StartCoinCollection(ICoin coin);

  // /// <summary>Inform the game that the player collected a coin.</summary>
  // /// <param name="coin">Coin that was collected.</param>
  // void OnFinishCoinCollection(ICoin coin);

  // /// <summary>Tells the game how many coins the game world contains.</summary>
  // /// <param name="numCoinsAtStart">Initial number of coins.</param>
  // void SetNumCoinsAtStart(int numCoinsAtStart);

  // /// <summary>Tells the game how many coins the player has collected.</summary>
  // /// <param name="numCoinsCollected">Number of coins collected.</param>
  // void SetNumCoinsCollected(int numCoinsCollected);

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
  ///  Sets the current counter the player is interacting with.
  /// </summary>
  /// <param name="counter"></param>
  void SetInteractableCounter(ICounter? counter);

  /// <summary>
  /// Sets the current counter the player is interacting with.
  /// </summary>
  /// <param name="counter"></param>
  void SetCurrentCounter(ICounter counter);
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

  public IAutoValue<ICounter?> InteractableCounter => _interactableCounter;
  private readonly AutoValue<ICounter?> _interactableCounter;

  public IAutoValue<ICounter> CurrentCounter => _currentCounter;
  private readonly AutoValue<ICounter> _currentCounter;

  private bool _disposedValue;

  public GameRepo()
  {
    _isMouseCaptured = new AutoValue<bool>(false);
    _isPaused = new AutoValue<bool>(false);
    _playerGlobalPosition = new AutoValue<Vector3>(Vector3.Zero);
    _cameraBasis = new AutoValue<Basis>(Basis.Identity);
    _interactableCounter = new AutoValue<ICounter?>(null);
    _currentCounter = new AutoValue<ICounter>(default!);
  }

  internal GameRepo(
    AutoValue<bool> isMouseCaptured,
    AutoValue<bool> isPaused,
    AutoValue<Vector3> playerGlobalPosition,
    AutoValue<Basis> cameraBasis,
    AutoValue<ICounter?> interactableCounter,
    AutoValue<ICounter> currentCounter
  )
  {
    _isMouseCaptured = isMouseCaptured;
    _isPaused = isPaused;
    _playerGlobalPosition = playerGlobalPosition;
    _cameraBasis = cameraBasis;
    _interactableCounter = interactableCounter;
    _currentCounter = currentCounter;
  }

  public void SetPlayerGlobalPosition(Vector3 playerGlobalPosition) =>
    _playerGlobalPosition.Value = playerGlobalPosition;

  public void SetIsMouseCaptured(bool isMouseCaptured) =>
    _isMouseCaptured.Value = isMouseCaptured;

  public void SetCameraBasis(Basis cameraBasis) =>
    _cameraBasis.Value = cameraBasis;

  public void SetInteractableCounter(ICounter? counter)
  {
    _interactableCounter.Value = counter;
    _autoChannel.Send(new IGameRepo.InteractableCounterChanged(counter));
  }

  public void SetCurrentCounter(ICounter counter)
  {
    _currentCounter.Value = counter;
    _autoChannel.Send(new IGameRepo.CurrentCounterChanged(counter));
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
        // _numCoinsCollected.Dispose();
        // _numCoinsAtStart.Dispose();
        _interactableCounter.Dispose();
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
