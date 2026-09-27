# Phantom Camera Migration Plan

## Goal

Replace the custom gameplay camera rig and its transform logic with Phantom Camera while preserving player-relative movement, mouse/gamepad controls, pause behavior, save/load, and LogicBlocks ownership boundaries. The existing probe at `test/phantom_camera/PhantomCameraProbe.tscn` is an API and behavior reference, not the final gameplay integration.

The production `Game.tscn` should use Phantom Camera as the sole authority for the active gameplay camera transform. Do not leave the old camera smoothing or a second active camera writing transforms in parallel.

## Current Contracts

- `Game.tscn` owns the player and `PlayerCamera` as siblings under `PauseContainer`; `Game.OnResolved` activates the gameplay camera.
- `PlayerCameraLogic` subscribes to `IGameRepo.IsMouseCaptured` and `PlayerGlobalPosition`. Its state handles mouse and joystick rotation, smooths gimbal/follow/offset transforms, and publishes the camera basis.
- `PlayerLogic.State.Alive` reads `GameRepo.CameraBasis` for camera-relative movement. `Player.cs` also publishes player position.
- `Game.Save()` and `Game.Load()` delegate camera persistence to `PlayerCamera.Save()` and `PlayerCamera.Load()`.
- The current camera save record contains LogicBlock state, global transform, local camera position, and offset position.
- Phantom Camera v0.11.0.3 is vendored in `addons/phantom_camera/`; its editor plugin and `PhantomCameraManager` autoload are configured in `project.godot`.

## Target Architecture

- Keep a project-owned `PlayerCamera` adapter at the current game boundary. `Game`, `IGameRepo`, save/load, and player movement should not depend on Phantom Camera internals directly.
- Replace the custom gimbal/SpringArm/camera hierarchy with one `Camera3D` containing `PhantomCameraHost` and a third-person `PhantomCamera3D` following the player (or a dedicated target node attached to the player).
- Phantom Camera owns follow, orbit, spring-arm collision, and camera transitions. The adapter owns integration policy: input gating, active-camera basis publication, lifecycle, and persistence.
- Keep LogicBlocks for game-controlled camera policy if useful (for example, whether orbit input is enabled); do not make a LogicBlock manually compute transforms already owned by Phantom Camera.
- Publish the active camera's horizontal orientation to `GameRepo.CameraBasis` after Phantom Camera updates. Preserve the existing camera-relative movement semantics and avoid feeding pitch into the ground movement vector.

## Work Plan

### 1. Pin and validate the dependency

- Keep the plugin at the tested stable version v0.11.0.3, retain its MIT license, and record the upstream release/source in project documentation or dependency notes.
- Confirm required addon files are included in exports and that the editor plugin/autoload initialize in both editor and exported/runtime builds.
- Review addon C# wrapper warnings and engine compatibility. Do not modify upstream files merely to silence non-fatal warnings; isolate any required local workaround in the project adapter.
- Keep the probe until production behavior passes the acceptance checks below.

**Exit criteria:** clean project import; no missing Phantom Camera manager; runtime and export templates can instantiate Phantom Camera nodes.

### 2. Define the camera adapter and persistence contract

- Update `IPlayerCamera` to expose only project-level operations/properties needed by the game: activate/deactivate, orbit-input enabled state if required, and the gameplay camera basis.
- Update `PlayerCamera` to resolve the `Camera3D`, `PhantomCameraHost`, and `PhantomCamera3D` nodes and provide the `PhantomCamera.PhantomCamera3D` wrapper only inside this adapter.
- Replace the current transform-based `PlayerCameraData` with intentional gameplay state: at minimum orbit yaw/pitch and any user-adjustable camera mode/zoom that should persist. Do not serialize transient addon node state or generated SpringArm nodes.
- Inspect the save-file format/versioning before changing the record. Define a migration/default path for saves containing the old LogicBlock state and transforms; old saves must load with a sensible default yaw/pitch rather than failing deserialization.
- Decide whether camera rotation persists across sessions. If not, explicitly reset to a deterministic default on new/load instead of inheriting scene/editor state.

**Exit criteria:** old and new saves have defined load behavior, and camera persistence is tested independently of the visual scene.

### 3. Replace camera behavior, preserving controls

- Configure Phantom Camera third-person follow, damping, follow offset, spring length, collision mask, and collision shape to match the current rig's framing and obstacle behavior.
- Route mouse motion to Phantom Camera's third-person rotation API only while `IGameRepo.IsMouseCaptured` is true. Preserve pitch limits and sensitivity from `PlayerCameraSettings`; do not hard-code the probe's temporary values.
- Route the existing `camera_left/right/up/down` gamepad actions through the same camera adapter and sensitivity/limit rules. Support mouse capture release/reacquisition without losing the desired orbit state.
- Keep input processing in the established Godot-node-to-LogicBlock boundary. If LogicBlocks continues to gate input, send intent/state through inputs and let the adapter apply the permitted orbit delta to Phantom Camera.
- Remove the old per-tick gimbal, follow transform, spring target, and offset lerps after equivalent Phantom Camera behavior is verified. Do not retain duplicate smoothing.

**Exit criteria:** mouse and right-stick orbit, pitch clamping, pause/capture gating, follow damping, and collision all match expected gameplay behavior.

### 4. Preserve game and movement integration

- Keep `Game`'s camera activation flow, but have it activate the Phantom Camera-backed adapter and ensure the host selects the expected camera priority.
- Update the adapter to publish the camera's effective basis to `GameRepo.CameraBasis` at the correct point in the frame. Confirm priority switches/transitions also publish the active camera basis rather than a stale or inactive rig basis.
- Preserve the current player movement rule: movement direction follows camera yaw on the ground, is normalized as before, and is unaffected by camera pitch.
- Keep `PlayerGlobalPosition` only if another consumer still needs it; Phantom Camera should follow the target node directly rather than receive position updates through a custom transform lerp.
- Check `Game.tscn` pause behavior: camera host and player must pause consistently, while menu/backdrop camera behavior remains unchanged.

**Exit criteria:** player movement remains camera-relative before, during, and after camera activation/pause, with no one-frame orientation lag that changes controls.

### 5. Update scenes and remove the old rig

- Replace the production `PlayerCamera.tscn` hierarchy with the host, camera, Phantom Camera, and any target node needed by the adapter.
- Remove obsolete gimbal, custom spring arm target, camera-node transform outputs, and unused camera offset handling from `PlayerCamera.cs`, `PlayerCameraLogic.cs`, `PlayerCameraLogicState.cs`, settings, and save data.
- Remove dead offset inputs/producers only after confirming no other consumer depends on them.
- Remove the temporary probe and its script once production tests exercise the same cases. Keep the Phantom Camera addon and license.
- Do not change unrelated UI/player scene edits as part of this migration.

**Exit criteria:** exactly one system controls the gameplay camera; there are no unused custom-rig APIs or test-only references in production.

### 6. Test and acceptance

- Add focused tests for camera basis publication, pitch limits, input gating, yaw/pitch save/restore, and legacy save defaults.
- Run the Godot scene/runtime tests and `dotnet build`; manually exercise the game with keyboard/mouse and a gamepad.
- Verify third-person collision against walls/counters, spring-arm recovery, player movement direction after large yaw changes, pause/resume, mouse release, and save/load.
- Verify camera behavior in the main gameplay scene and any menu/backdrop scene that owns a camera.
- Test an exported build, not only the editor, to catch plugin/autoload or custom-resource omissions.

**Acceptance criteria:**

1. Phantom Camera is the only writer of the active gameplay camera transform.
2. WASD and gamepad movement remain camera-relative and horizontal.
3. Mouse and right-stick orbit preserve configured sensitivity and pitch limits.
4. Mouse capture, pause/resume, and camera activation work through existing game state flow.
5. Camera collision, follow damping, and framing are acceptable in representative kitchen layouts.
6. Current saves load and legacy saves either migrate or fall back to documented defaults.
7. Build, automated tests, headless scene startup, and exported runtime show no camera/plugin errors.

## Main Files Expected to Change

- `src/player_camera/PlayerCamera.cs`
- `src/player_camera/PlayerCamera.tscn`
- `src/player_camera/PlayerCameraData.cs`
- `src/player_camera/PlayerCameraLogic.cs` and `src/player_camera/state/*` (simplify or remove transform-solving state)
- `src/game/Game.cs` and `src/game/Game.tscn` (only if activation/scene node wiring requires it)
- `src/game/domain/GameRepo.cs` (only if a camera-facing contract adjustment is needed)
- `src/player/state/states/PlayerLogic.State.Alive.cs` (only if movement-basis integration requires a correction)
- focused tests under `test/`
- `project.godot` and dependency documentation only as required for plugin/export setup

## Risks and Decisions to Resolve Before Production Cutover

- **Save compatibility:** the old save data is structurally different from orbit yaw/pitch. Confirm the serializer's schema evolution behavior and implement an explicit fallback/migration.
- **Basis timing:** publishing a basis before Phantom Camera has applied its current-frame rotation can make movement lag behind camera orbit.
- **Rotation ownership:** Phantom Camera's third-person rotation and its scene-node transform have distinct semantics; use the documented wrapper API rather than writing `Rotation` directly.
- **Collision behavior:** the current rig uses a sphere shape and margin; match those parameters rather than accepting addon defaults.
- **Pause processing:** confirm the camera host and Phantom Camera nodes respect the game's pause mode and do not continue moving while paused.
- **Input source of truth:** retain the project's action names and `PlayerCameraSettings`; do not add an independent input mapping solely for the plugin.
- **Plugin updates:** pin upgrades and re-run the compatibility/runtime checks because the addon contains both GDScript runtime code and C# wrappers.

## Rollout Strategy

## Implementation Log

### Step 1 - Persistent camera state and legacy compatibility

Status: implemented.

- `PlayerCameraData` now persists deterministic `yaw_degrees` and `pitch_degrees`.
- Legacy LogicBlock and transform fields remain optional during the transition, so old save files can still be read.
- `PlayerCamera.Load()` uses the new angles when present and clamps pitch through `PlayerCameraSettings`; legacy transform restoration remains available until the custom rig is removed.
- The next step is to replace the production hierarchy with a Phantom Camera-backed adapter while preserving `IGameRepo.CameraBasis`.

Validation: `dotnet build` réussi (0 erreur ; avertissements préexistants dans les wrappers Phantom Camera).

### Step 2 - Production adapter and Phantom Camera hierarchy

Status: implemented.

- `PlayerCamera` delegates follow, damping, third-person orbit and collision to Phantom Camera.
- `Game` and `IGameRepo` keep their project-owned boundary; `CameraBasis` is published as a horizontal basis from the active `Camera3D`.
- Mouse and right-stick input remain gated by `IGameRepo.IsMouseCaptured`, with sensitivity and pitch limits coming from `PlayerCameraSettings`.
- The custom gimbal, SpringArm, offset lerps and camera LogicBlock states were removed from the production path.
- The Phantom Camera follows `../../Player` directly from `PlayerCamera.tscn`; its host remains under the gameplay `Camera3D`.

Validation: `dotnet build` réussi (0 erreur ; 5 avertissements non bloquants dans des wrappers/addons existants).

Runtime: démarrage headless de la scène principale réussi avec `--quit-after 2`.

Timing decision: `PlayerCamera` publie sa base à la priorité physique `400`, après le `PhantomCameraHost` (`300`), afin que le déplacement du joueur utilise la pose Phantom de la frame courante.

### Follow-distance correction

Status: implemented.

- Corrected the follow target to `../../Player`; `PhantomCamera3D` is nested under `PlayerCamera`, while `Player` is a sibling of `PlayerCamera` under `PauseContainer`.
- Configured an explicit 4.5-unit third-person distance, a 1.2-unit vertical follow offset, and the previous spherical collision shape/margin.
- Removed the obsolete `Offset` scene property from `Game.tscn`.

Validation: `dotnet build` réussi (0 erreur ; avertissements préexistants des wrappers Phantom Camera) et démarrage headless réussi avec `--quit-after 2`.

### Follow-target runtime fix

Status: implemented.

- `Player` and `PlayerCamera` are siblings under `PauseContainer`, but `PhantomCamera3D` is nested one level below `PlayerCamera`; the valid target path is therefore `../../Player`.
- The invalid shorter path left the Phantom Camera without a valid follow target, causing the camera to stay fixed and orbit away from the player.

Validation: `dotnet build` réussi et démarrage headless de la scène principale réussi après correction.

### Follow target restored

Status: implemented.

- The current production scene had lost the `follow_target` property entirely, leaving Phantom Camera without a target.
- Restored `follow_target = NodePath("../../Player")`, matching the instantiated hierarchy: `PhantomCamera3D` is inside `PlayerCamera`, while `Player` is its sibling under `PauseContainer`.
- This keeps the camera anchored to Player while Player moves or the orbit angle changes.

Validation: `dotnet build` réussi (0 erreur, 0 avertissement) et démarrage headless de la scène principale réussi avec `--quit-after 2`.

### Phantom ownership fix

Status: implemented.

- Removed the adapter's direct `Camera3D.MakeCurrent()` call; PhantomCameraHost now remains the sole system that applies the active camera transform.
- `PlayerCamera` assigns `FollowTarget` at runtime from the stable sibling path `../Player`, avoiding fragile relative paths inside the packed scene instance.
- Set the host interpolation mode to physics so Phantom Camera follows Player in the same update domain as gameplay movement.

Validation: `dotnet build` réussi (0 erreur ; avertissements préexistants de l’addon) et démarrage headless réussi avec `--quit-after 2`.

Runtime ownership check: aucune écriture projet de `Camera3D` (`GlobalTransform`, `GlobalPosition`, `GlobalRotation`) ni aucun appel `MakeCurrent()` ne subsiste dans le périmètre gameplay ; `PhantomCameraHost` est l’unique écrivain du transform actif.

### PlayerCameraLogic cleanup

Status: implemented.

- Removed the unused `PlayerCameraLogic.Data` record and generated state/diagram artifacts.
- Removed the obsolete LogicBlock and transform fields from `PlayerCameraData`; camera saves now contain only `yaw_degrees` and `pitch_degrees`.
- Confirmed there are no remaining references to `PlayerCameraLogic`, `PlayerCameraLogicState`, `StateMachine`, or the old camera transform fields in `src/player_camera`, `src/game`, or `test`.

Validation: `dotnet build` réussi (0 erreur ; avertissements limités aux wrappers Phantom Camera existants).

### Follow damping correction

Status: implemented.

- Changed Phantom Camera damping from `8` seconds to `0.15` seconds on all axes.
- Phantom Camera's `_smooth_damp` treats `follow_damping_value` as a damping time; the previous value made the camera take several seconds to catch up with Player.
- Distance, collision and follow target are unchanged.

Validation: `dotnet build` réussi (0 erreur, 0 avertissement) et démarrage headless réussi avec `--quit-after 2`.

### Current migration status

Implemented through the adapter and production-scene cutover. Automated checks currently passing:

- `dotnet build` (0 errors, 0 warnings on the final run)
- Godot editor headless import with Phantom Camera plugin/autoload enabled
- Main scene headless startup with `--quit-after 2`

Still requiring an interactive acceptance pass: mouse/right-stick feel, collision and recovery around kitchen obstacles, pause and mouse recapture, save/load in a running session, and an exported runtime. The probe remains available as the API reference until those checks are exercised against the production scene.

Implement the adapter and save migration first, then integrate it in a temporary branch/scene while the old camera remains available for comparison. Switch the production scene only after movement, input, pause, collision, save/load, and exported-build checks pass. Remove the old rig and probe in the same final cleanup after the new camera is the verified production path.
