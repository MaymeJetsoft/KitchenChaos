namespace KitchenChaos;

public partial class PlayerLogic
{
  /// <summary>Player settings.</summary>
  /// <param name="RotationSpeed">Rotation speed (quaternions?/sec).</param>
  /// <param name="StoppingSpeed">Stopping velocity (meters/sec).</param>
  /// <param name="Gravity">Player gravity (meters/sec).</param>
  /// <param name="MoveSpeed">Player speed (meters/sec).</param>
  /// <param name="Acceleration">Player speed (meters^2/sec).</param>
  public record Settings(
    float RotationSpeed,
    float StoppingSpeed,
    float Gravity,
    float MoveSpeed,
    float Acceleration
  );
}
