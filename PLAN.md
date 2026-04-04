# Plan: Boardball Web Application — Technical Implementation

## Context
The game rules are fully documented in `GAME RULES.md`. The goal is to implement Boardball as a web application: .NET 9 + C# backend (game logic + REST API) and Vue.js frontend. Two players share a single browser tab — no real-time multiplayer or authentication needed.

---

## 1. Solution & Project Structure

```
boardball/
├── BoardBall.sln
├── GAME RULES.md
├── CLAUDE.md
│
├── BoardBall.Core/                    # Domain logic — C# class library
│   ├── BoardBall.Core.csproj
│   ├── GameConfig.cs
│   ├── Game.cs
│   ├── State.cs
│   ├── Commands.cs
│   ├── Events.cs
│   ├── JumpEngine.cs
│   └── Enums.cs                       # Player, Direction, GameStatus
│
├── BoardBall.Core.Tests/              # xUnit + Shouldly test project
│   ├── BoardBall.Core.Tests.csproj
│   ├── GameConfigTests.cs
│   ├── GameStartTests.cs
│   ├── PlaceStoneTests.cs
│   ├── JumpTests.cs
│   ├── ChainJumpTests.cs
│   ├── WinDetectionTests.cs
│   └── DrawDetectionTests.cs
│
├── BoardBall.Api/                     # ASP.NET Core Web API
│   ├── BoardBall.Api.csproj
│   ├── Program.cs
│   ├── Controllers/
│   │   └── GameController.cs
│   └── Models/
│       ├── StartGameRequest.cs
│       ├── PlaceStoneRequest.cs
│       ├── JumpRequest.cs
│       ├── SkipRequest.cs
│       └── GameStateResponse.cs
│
└── boardball-ui/                      # Vue.js 3 + Vite frontend
    ├── package.json
    ├── vite.config.ts
    ├── tsconfig.json
    ├── index.html
    └── src/
        ├── main.ts
        ├── App.vue
        ├── types/game.ts
        ├── api/gameApi.ts
        ├── stores/gameStore.ts        # Pinia store
        └── components/
            ├── GameSetup.vue
            ├── GameBoard.vue
            ├── BoardCell.vue
            ├── GameStatus.vue
            └── ActionPanel.vue
```

---

## 2. Core Domain Model

### Conventions
- Matches prior codebase: `System.Drawing.Point` for coordinates (1-based, X=column, Y=row)
- Error keys are `const string` fields on the classes (e.g. `Game.WrongPlayer`)
- `ArgumentException(key)` is the error mechanism; API layer catches uniformly
- Test structure: `//Given //When //Then`, `Test_` prefix, Shouldly assertions

---

### `Enums.cs`
```csharp
public enum Player     { One, Two }
public enum GameStatus { NotStarted, InProgress, Won, Draw }
public enum Direction  { Up, Down, Left, Right, UpLeft, UpRight, DownLeft, DownRight }
```

---

### `GameConfig.cs`
```csharp
public record GameConfig(string Player1, string Player2, int Rows, int Columns, int StackSize)
{
    // Validation in constructor: throws ArgumentException(key) on failure
    // Keys: "player1", "player2", "rows", "columns", "stack"
    // Rules:
    //   player1/2: not null or empty
    //   rows: odd, >= 1
    //   columns: odd, >= 5
    //   stackSize: > 0

    public int CenterRow    => (Rows    / 2) + 1;   // 1-based
    public int CenterColumn => (Columns / 2) + 1;   // 1-based
    public int Player1GoalColumn => Columns;         // Player1 attacks the LAST column
    public int Player2GoalColumn => 1;               // Player2 attacks the FIRST column
}
```

---

### `Commands.cs`
```csharp
public static class Commands
{
    public record StartGame(GameConfig Config);
    public record PlaceStone(Player Player, Point Location);
    public record JumpBall(Player Player, Direction Direction);
    public record SkipTurn(Player Player);
}
```

---

### `Events.cs`
```csharp
public abstract record Event;

public static class Events
{
    public record GameStarted(GameConfig Config)                                : Event;
    public record BallPlaced(Point Location)                                   : Event;
    public record CurrentPlayerChanged(Player Player)                          : Event;
    public record StonePlaced(Point Location)                                  : Event;
    public record BallMoved(Point From, Point To)                              : Event;
    public record StonesReturned(IReadOnlyList<Point> Locations)               : Event;
    public record TurnSkipped(Player Player)                                   : Event;
    public record ChainContinuationRequired(IReadOnlyList<Direction> Available): Event;
    public record GameWon(Player Winner)                                       : Event;
    public record GameDrawn()                                                  : Event;
}
```

---

### `State.cs`
Applied by consuming events. Exposes current board snapshot.

```csharp
public class State
{
    public GameConfig?    Config           { get; private set; }
    public Player?        CurrentPlayer    { get; private set; }
    public Point          Ball             { get; private set; }
    public HashSet<Point> Stones           { get; private set; } = new();
    public int            StackCount       { get; private set; }
    public GameStatus     Status           { get; private set; } = GameStatus.NotStarted;
    public Player?        Winner           { get; private set; }
    public int            ConsecutiveSkips { get; private set; }
    public bool           ChainInProgress  { get; private set; }

    public void Apply(IEnumerable<Event> events) { /* pattern-match and mutate */ }
}
```

Key apply rules:
- `StonePlaced`: add to Stones, decrement StackCount, reset ConsecutiveSkips
- `BallMoved`: update Ball, reset ConsecutiveSkips
- `StonesReturned`: remove from Stones, increment StackCount
- `TurnSkipped`: increment ConsecutiveSkips
- `CurrentPlayerChanged`: switch player, set ChainInProgress = false
- `ChainContinuationRequired`: set ChainInProgress = true
- `GameWon` / `GameDrawn`: set Status + Winner

---

### `Game.cs`
Main aggregate. Receives Commands → validates → emits Events → applies to State.

```csharp
public class Game
{
    // Error keys
    public const string GameAlreadyStarted = "started";
    public const string GameNotStarted     = "start";
    public const string WrongPlayer        = "player";
    public const string StackEmpty         = "stack";
    public const string LocationOccupied   = "occupied";
    public const string OutOfBounds        = "bounds";
    public const string InvalidJump        = "jump";
    public const string MustContinueChain  = "chain";
    public const string GameOver           = "gameover";
    public const string CannotSkip         = "skip";

    public State State { get; } = new();

    public IEnumerable<Event> Handle(Commands.StartGame cmd)  { ... }
    public IEnumerable<Event> Handle(Commands.PlaceStone cmd) { ... }
    public IEnumerable<Event> Handle(Commands.JumpBall cmd)   { ... }
    public IEnumerable<Event> Handle(Commands.SkipTurn cmd)   { ... }
}
```

**`Handle(StartGame)`:**
- Guard: NotStarted
- Emit: `GameStarted`, `BallPlaced(center)`, `CurrentPlayerChanged(Player.One)`

**`Handle(PlaceStone)`:**
- Guards: InProgress, correct player, not ChainInProgress, stack > 0, in-bounds, not ball, not occupied
- Emit: `StonePlaced`, then `CurrentPlayerChanged(other)` or auto-skip if needed

**`Handle(JumpBall)`:**
- Guards: InProgress, correct player (same player must continue chain)
- Call `JumpEngine.TryHop(State, direction)` → null → throw `InvalidJump`
- Emit: `BallMoved`, `StonesReturned`
- Check `IsGoal(landing)` → if true: emit `GameWon(CurrentPlayer)`, return
- Check `JumpEngine.GetAvailableDirections(State)` → if any: emit `ChainContinuationRequired`
- Else: emit `CurrentPlayerChanged(other)`, run auto-skip check

**`Handle(SkipTurn)`:**
- Guards: InProgress, correct player, not ChainInProgress
- Validate: StackCount == 0 AND !JumpEngine.HasAnyValidJump(State) → else throw `CannotSkip`
- Emit: `TurnSkipped`
- If ConsecutiveSkips >= 2 after apply: emit `GameDrawn`
- Else: emit `CurrentPlayerChanged(other)`, run auto-skip check on other player

**Auto-skip check (private helper):**
After every turn change, if next player also has no moves: emit `TurnSkipped` for them.
If ConsecutiveSkips >= 2: emit `GameDrawn` instead of continuing.

---

### `JumpEngine.cs` (static, pure)

```csharp
public static class JumpEngine
{
    public static HopResult? TryHop(State state, Direction direction) { ... }
    public static IReadOnlyList<Direction> GetAvailableDirections(State state) { ... }
    public static bool HasAnyValidJump(State state) { ... }
}

public record HopResult(Point Landing, IReadOnlyList<Point> JumpedStones, bool IsGoal);
```

**`TryHop` algorithm:**
```
(dr, dc) = direction vector (e.g. Up = (-1, 0), UpRight = (-1, +1))
cursor = ball + (dr, dc)
stones = []

// Collect consecutive stones
while cursor is on-board AND cursor ∈ state.Stones:
    stones.Add(cursor)
    cursor += (dr, dc)

if stones.Count == 0: return null    // must jump at least one stone

landing = cursor

if landing is on-board (row ∈ [1..Rows], col ∈ [1..Columns]):
    if landing ∈ state.Stones: return null   // occupied
    return HopResult(landing, stones, IsGoal(landing))
else:
    if dc == 0: return null                                   // exited top or bottom — invalid
    if landing.row < 1 || landing.row > Rows: return null     // diagonal off top/bottom — invalid
    // Horizontal or diagonal exit past left/right goal line → valid goal
    return HopResult(landing, stones, isGoal: true)
```

**`IsGoal` (position-based, not player-based):**
```
isGoal = col <= config.Player2GoalColumn   // col <= 1  → Player 2 wins
       || col >= config.Player1GoalColumn  // col >= m  → Player 1 wins
```
Note: "own goal" is possible — if the ball lands on the current player's defended column, the opponent wins.

---

## 3. REST API

**Base route:** `/api/game`

| Method | Path | Body | Description |
|--------|------|------|-------------|
| `POST` | `/start` | `StartGameRequest` | Start / restart a game |
| `GET`  | `/state` | — | Get current state |
| `POST` | `/place-stone` | `PlaceStoneRequest` | Place a stone |
| `POST` | `/jump` | `JumpRequest` | Make one hop |
| `POST` | `/skip` | `SkipRequest` | Skip turn |

All actions return `GameStateResponse` on success or `{ "error": "<key>" }` on `400 Bad Request`.

### Request DTOs
```csharp
record StartGameRequest(string Player1, string Player2, int Rows, int Columns, int StackSize);
record PlaceStoneRequest(Player Player, int Row, int Column);
record JumpRequest(Player Player, Direction Direction);
record SkipRequest(Player Player);
```

### `GameStateResponse`
```csharp
record GameStateResponse(
    string         Status,                  // "NotStarted"|"InProgress"|"Won"|"Draw"
    string?        Winner,                  // "One"|"Two"|null
    string?        CurrentPlayer,
    string?        Player1Name,
    string?        Player2Name,
    int            Rows,
    int            Columns,
    int            StackCount,
    PointDto       Ball,
    List<PointDto> Stones,
    bool           ChainInProgress,
    List<string>   AvailableJumpDirections,
    bool           CanPlaceStone,
    bool           CanJump,
    bool           MustSkip,
    int            ConsecutiveSkips
);
record PointDto(int Row, int Column);
```

The response is intentionally fat — one call returns everything the frontend needs to render.

### `Program.cs` (key registrations)
```csharp
builder.Services.AddSingleton<Game>();    // single in-memory game instance
builder.Services.AddCors(...);            // allow http://localhost:5173
```

### Error handling in controller
```csharp
catch (ArgumentException ex) => BadRequest(new { error = ex.Message })
```

---

## 4. Vue.js Frontend

**Stack:** Vue 3 + Vite + TypeScript + Pinia + Axios

### Component Tree
```
App.vue
├── GameSetup.vue       (status === "NotStarted")
│   └── Form: player1, player2, rows(15), columns(19), stack(30)
└── GameBoard.vue       (status !== "NotStarted")
    ├── GameStatus.vue  (current player banner / result overlay)
    ├── BoardCell.vue   (n×m grid cells; renders ball/stone/empty)
    └── ActionPanel.vue (Skip button; direction arrows during chain)
```

### Pinia Store (`gameStore.ts`)
```typescript
state: { game: GameState | null; error: string | null; loading: boolean }
actions: startGame, refreshState, placeStone, jump, skipTurn, clearError
```
Each action calls the API, sets `game` to the returned state, sets `error` on failure.

### Key TypeScript types (`types/game.ts`)
```typescript
type PlayerKey     = 'One' | 'Two';
type GameStatusKey = 'NotStarted' | 'InProgress' | 'Won' | 'Draw';
type DirectionKey  = 'Up'|'Down'|'Left'|'Right'|'UpLeft'|'UpRight'|'DownLeft'|'DownRight';

interface GameState {
  status: GameStatusKey;
  winner: PlayerKey | null;
  currentPlayer: PlayerKey | null;
  player1Name: string | null;
  player2Name: string | null;
  rows: number;
  columns: number;
  stackCount: number;
  ball: { row: number; column: number };
  stones: { row: number; column: number }[];
  chainInProgress: boolean;
  availableJumpDirections: DirectionKey[];
  canPlaceStone: boolean;
  canJump: boolean;
  mustSkip: boolean;
  consecutiveSkips: number;
}
```

### `vite.config.ts` proxy
```typescript
server: { proxy: { '/api': { target: 'http://localhost:5000', changeOrigin: true } } }
```

---

## 5. Game Flow (Frontend ↔ Backend)

```
1. Setup screen → POST /start → GameStateResponse → render board

2. Player clicks empty cell:
   POST /place-stone → response → board re-renders, turn changes

3. Player clicks directional arrow:
   POST /jump → response:
     chainInProgress=true  → highlight available directions, lock other actions
     chainInProgress=false → turn changes normally

4. Player clicks "Skip Turn" (only shown when mustSkip=true):
   POST /skip → response → turn changes or draw declared

5. Win/Draw: GameStatus.vue shows overlay → "New Game" → back to setup
```

No polling. Every action returns full state in the HTTP response.

---

## 6. Development Setup

**Backend** (port 5000):
```bash
dotnet run --project BoardBall.Api
```

**Frontend** (port 5173, proxies /api to port 5000):
```bash
cd boardball-ui && npm install && npm run dev
```

**Tests:**
```bash
dotnet test BoardBall.Core.Tests
```

---

## 7. Test Coverage Plan

Test conventions (from prior codebase):
- `Test_` prefix, `//Given //When //Then`, Shouldly, no mocks

### `GameConfigTests` — validation of all config parameters
### `GameStartTests` — ball at center, Player1 first, stack set, can't start twice
### `PlaceStoneTests` — placement guards, goal line allowed, stack decrement, alternation
### `JumpTests` — all 8 directions, multi-stone hops, invalid hops, stones returned
### `ChainJumpTests` — mandatory continuation, direction change, revisit, chain-broken-by-goal
### `WinDetectionTests` — reach goal col, off-board goal, mid-chain win, post-win lockout
### `DrawDetectionTests` — 2 consecutive skips, invalid skip (stack not empty / jump available), counter resets

Full test matrix: ~50 test cases total across all files.

---

## Critical Files to Create

All files are new (directory was cleared):
- `BoardBall.Core/`: `GameConfig.cs`, `Game.cs`, `State.cs`, `Commands.cs`, `Events.cs`, `JumpEngine.cs`, `Enums.cs`
- `BoardBall.Core.Tests/`: 7 test files
- `BoardBall.Api/`: `Program.cs`, `GameController.cs`, 5 request/response DTOs
- `boardball-ui/src/`: `App.vue`, `gameStore.ts`, `gameApi.ts`, `types/game.ts`, 5 components

## Verification
1. `dotnet test` — all Core tests pass
2. `dotnet run` for API + `npm run dev` for UI
3. Play a complete game in the browser: setup → place stones → jump → win
4. Verify chain jumps force continuation
5. Verify draw: deplete stack, no valid jumps, two skips
6. Verify off-board goal scoring
