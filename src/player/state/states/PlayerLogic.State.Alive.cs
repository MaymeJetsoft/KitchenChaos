namespace KitchenChaos;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;

public partial record PlayerLogicState
{
  private const float MOVEMENT = 0.2f;

  [Meta]
  public abstract partial record Alive : PlayerLogicState,
    IGet<Input.PhysicsTick>,
    IGet<Input.Moved>,
    IGet<Input.Pushed>,
    IGet<Input.Killed>,
    IGet<Input.FacingCounterChanged>
  {
    // Movement is allowed in any state (even in the air), so these inputs
    // handle movement for each substate unless overridden.
    //
    // If you needed different mechanics, you could move these handlers to
    // a MoveEnabled substate and extend it for states where movement is
    // allowed.

    public virtual Type On(in Input.Killed input)
    {
      Get<IGameRepo>().OnGameEnded(GameOverReason.Lost);

      return To<Dead>();
    }

    public virtual Type On(in Input.FacingCounterChanged input)
    {
      Get<IGameRepo>().SetFacingCounter(input.Counter);
      return ToSelf();
    }

    public virtual Type On(in Input.PhysicsTick input)
    {
      var delta = input.Delta;
      var player = Get<IPlayer>();
      var settings = Get<PlayerLogic.Settings>();
      var gameRepo = Get<IGameRepo>();
      var data = Get<PlayerLogic.Data>();

      var cameraBasis = gameRepo.CameraBasis.Value;
      var moveDirection = player.GetGlobalInputVector(cameraBasis);

      var direction = new Vector2(moveDirection.X, moveDirection.Z);


      if (moveDirection.Length() > MOVEMENT)
      {
        data.LastStrongDirection = moveDirection.Normalized();
      }
      else
      {
        direction = new Vector2(data.LastStrongDirection.X, data.LastStrongDirection.Z);
      }

      var nextRotationBasis = player.GetNextRotationBasis(
        data.LastStrongDirection, delta, settings.RotationSpeed
      );

      data.LastVelocity = player.Velocity;
      var velocity = data.LastVelocity with { Y = 0f };
      velocity = velocity.Lerp(
        moveDirection * settings.MoveSpeed,
        settings.Acceleration * (float)delta
      );

      if (
        moveDirection.Length() == 0f &&
        velocity.Length() < settings.StoppingSpeed
      )
      {
        velocity = Vector3.Zero;
      }

      // Don't clear out Y velocity (in case we're falling/jumping, etc).
      velocity.Y = data.LastVelocity.Y;

      // Add gravity.
      velocity.Y += settings.Gravity * (float)delta;

      Output(
        new Output.MovementComputed(
          nextRotationBasis, velocity, direction, delta
        )
      );

      return ToSelf();
    }

    public virtual Type On(in Input.Moved input)
    {
      var player = Get<IPlayer>();
      var settings = Get<PlayerLogic.Settings>();
      var gameRepo = Get<IGameRepo>();
      var data = Get<PlayerLogic.Data>();

      // Tell the game the player has moved.
      // Anything that subscribes to our position (like the camera) will
      // be updated.
      gameRepo.SetPlayerGlobalPosition(input.GlobalPosition);

      var isMovingHorizontally = player.IsMovingHorizontally();
      var isOnFloor = player.IsOnFloor();
      // var hasNegativeYVelocity = player.Velocity.Y < 0f;

      // Determine whether we've *just* encountered a condition that would
      // be relevant to a state change.

      // Leaving the ground is more important than whether or not we've started
      // moving horizontally, since our grounded/airborne states are parent
      // states at the same level of the state hierarchy. Also, horizontal
      // movement is not important for the airborne state, since both jumping
      // and falling allow for horizontal movement, whereas being idle on the
      // ground is only possible while not moving horizontally.
      //
      //         +----------------+
      //   +-----|    Grounded    |----+
      //   |     +----------------+    |
      //   |  +------+     +--------+  |
      //   +->| Idle |     | Moving |<-+
      //      +------+     +--------+

      var justStartedMovingHorizontally =
        isMovingHorizontally && !data.WasMovingHorizontally(settings);
      var justStoppedMovingHorizontally =
        !isMovingHorizontally && data.WasMovingHorizontally(settings);

      // Update the cached values so we can use them next frame.
      data.WasOnFloor = isOnFloor;
      data.LastVelocity = player.Velocity;

      // Grounded status hasn't changed. Check for changes in horizontal
      // movement.

      if (justStartedMovingHorizontally)
      {
        Input(new Input.StartedMovingHorizontally());
      }
      else if (justStoppedMovingHorizontally)
      {
        Input(new Input.StoppedMovingHorizontally());
      }

      return ToSelf();
    }

    public Type On(in Input.Pushed input)
    {
      var player = Get<IPlayer>();
      var velocity = player.Velocity;

      // Apply force
      velocity += input.GlobalForceImpulseVector;
      Output(new Output.VelocityChanged(velocity));

      // Remain in current state. Next physics tick will end up applying the
      // force which will make us re-evaluate our state in On(in Input.Moved)
      return ToSelf();
    }
  }
}
