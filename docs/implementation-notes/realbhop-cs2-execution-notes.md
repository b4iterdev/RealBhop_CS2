# RealBhop CS2 Execution Notes

Date: 2026-05-21

## Workspace notes

- Workspace path: `/Users/b4iterdev/RealBhop_CS2`
- Git status: repository initialized on branch `main`; no commits have been created.

## Obstacles

- The workspace is not a git repository. The `using-git-worktrees` workflow cannot create an isolated git worktree, so changes are being made in-place.
- `.NET SDK` is not installed or not on `PATH`: running `dotnet --info` returned `zsh:1: command not found: dotnet`. This blocks TDD verification (`dotnet test`) and build verification.
- Homebrew is available, so I installed the `dotnet` formula to unblock TDD/build verification.
- The default Homebrew `dotnet` formula installed SDK 10, whose templates rejected `--framework net8.0`. I installed `dotnet@8` as a keg-only SDK and will run it explicitly from `/opt/homebrew/opt/dotnet@8/bin/dotnet`.
- C# LSP diagnostics could not run because `csharp-ls` is not installed. Attempting `dotnet tool install -g csharp-ls` failed with `DotnetToolSettings.xml was not found in the package`, so build/test output is the current verification source.
- Git was initialized with `GIT_MASTER=1 git init` after the user requested repo initialization.
- `dotnet@8` is linked as the active Homebrew `dotnet`; `dotnet --info` reports SDK `8.0.125`.
- `csharp-ls` latest targets .NET 10 and failed through the dotnet tool path under the previous setup. I installed/published a .NET 8-compatible `csharp-ls` 0.16.0 launcher at `/Users/b4iterdev/.local/bin/csharp-ls`, and LSP diagnostics now run cleanly.

## Trade-offs

- I saved the plan and notes in-place because there is no git repository/worktree available. This preserves progress but does not provide branch isolation.
- I am stopping before creating production code because the TDD skill requires watching tests fail before implementation; without `dotnet`, I cannot verify the RED step.
- Installing `dotnet` via Homebrew installed the current formula version (`10.0.107`) rather than the original minimum target (.NET 8). The project can still target `net8.0`, but SDK behavior/tooling will come from .NET 10.
- I removed the `.slnx` created by the failed SDK 10 attempt and will recreate the solution with SDK 8 tooling.
- I kept movement physics and state-machine logic independent from CounterStrikeSharp API types (`MovementVector`, `MovementSnapshot`) so unit tests can run without a CS2 server and without mocking engine objects. The trade-off is an adapter layer is still needed to convert live CS2 pawn/controller state into snapshots and corrections back into engine velocity writes.
- The plugin skeleton currently registers load/unload/tick/disconnect listeners but does not yet mutate live player velocity. This is intentional for the first executable checkpoint: tested pure logic first, CS2 API binding next.
- I added `global.json` pinned to SDK `8.0.125` so this workspace consistently uses .NET 8 even if newer SDKs are installed.
- I added `.sisyphus/` to `.gitignore` to avoid committing local continuation/session state.
- I kept the manual `csharp-ls` launcher in `~/.local/bin` because the dotnet global tool apphost needs shell `DOTNET_ROOT` configuration to run directly, while the launcher works in this environment and satisfies the LSP command requirement.

## Attention needed

- No current attention required for local build/test/LSP. Remaining attention may be needed later for CounterStrikeSharp runtime/server validation.

## Verification log

- `dotnet test tests/RealBhopCS2.Tests/RealBhopCS2.Tests.csproj --no-restore` initially failed after adding `BhopPhysicsTests` because `RealBhopCS2.Config` and `RealBhopCS2.Movement` did not exist. This verified the RED step.
- `dotnet test tests/RealBhopCS2.Tests/RealBhopCS2.Tests.csproj --no-restore` failed after adding the ground-tick penalty test because correction remained `50` instead of expected `48.75`. This verified the RED step for penalty support.
- `dotnet test tests/RealBhopCS2.Tests/RealBhopCS2.Tests.csproj --no-restore` failed after adding the max-correction clamp test because horizontal length remained `500` instead of expected `100`. This verified the RED step for clamp support.
- `dotnet test tests/RealBhopCS2.Tests/RealBhopCS2.Tests.csproj --no-restore` passed after implementing correction math: 4 tests passed.
- `dotnet test tests/RealBhopCS2.Tests/RealBhopCS2.Tests.csproj --no-restore` failed after adding `BhopTrackerTests` because `RealBhopCS2.Tracking` did not exist. This verified the RED step for the tracker.
- `dotnet test tests/RealBhopCS2.Tests/RealBhopCS2.Tests.csproj --no-restore` passed after adding `BhopTracker`: 5 tests passed.
- `dotnet build RealBhopCS2.sln --no-restore` passed: 0 warnings, 0 errors.
- `dotnet test RealBhopCS2.sln --no-restore` passed: 5 tests passed.
- `dotnet restore RealBhopCS2.sln` passed after cache clearing.
- `dotnet build RealBhopCS2.sln --no-restore` passed after linking `dotnet@8`: 0 warnings, 0 errors.
- `dotnet test RealBhopCS2.sln --no-restore` passed after linking `dotnet@8`: 5 tests passed.
- `lsp_diagnostics` for `src/RealBhopCS2` passed: 0 diagnostics.
- `lsp_diagnostics` for `tests/RealBhopCS2.Tests` passed: 0 diagnostics.
- `GIT_MASTER=1 git status --short` shows uncommitted project files ready for review/staging; no commit was made because the user did not request one.
