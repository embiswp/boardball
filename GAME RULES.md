# Boardball — Game Rules

## 1. Overview

Boardball is a two-player turn-based strategy game played on a rectangular grid. Players take turns placing stones and jumping a ball across the board. The objective is to move the ball to the opponent's goal line.

## 2. Setup

### Board
- Rectangular grid of **n rows x m columns**.
- **n** must be odd and greater or equal than 1 (minimum: 1).
- **m** must be odd and greater or equal than 5 (minimum: 5).
- Minimum board size: **1 x 5**.

### Ball
- The ball starts on the **center square**: row `ceil(n/2)`, column `ceil(m/2)`.

### Stone Stack
- A **shared pool** of stones is used by both players.
- The number of stones must be **configured before the game starts** (there is no default).

### Players and Goal Lines
- **Player 1** defends the **first column** (left edge).
- **Player 2** defends the **last column** (right edge).
- Each player's objective is to move the ball to the **opponent's** goal line:
  - Player 1 tries to reach the **last column** (or off-board to its right).
  - Player 2 tries to reach the **first column** (or off-board to its left).

### First Turn
- **Player 1** takes the first turn.

## 3. Turns

Players alternate turns. On each turn, a player **must** do exactly one of the following:

### Option A: Place a Stone
- Take one stone from the shared stack (the stack must not be empty).
- Place it on **any empty square** on the board, including goal line squares.
- The ball's square is **not** considered empty.

### Option B: Jump the Ball
- Execute a jump chain (see Section 4).

### Skipped Turn
- If the shared stack is empty **and** no valid jump exists, the player's turn is **skipped**.

## 4. Jumping Mechanics

### 4a. Single Hop

A single hop moves the ball in one of **8 directions**: up, down, left, right, or any of the 4 diagonals.

To perform a hop:
1. There must be **one or more consecutive stones** in a straight line, starting from the square adjacent to the ball in the chosen direction.
2. The ball leaps over the **entire line** of consecutive stones.
3. The ball lands on the **first square immediately after the last stone** in the line.
4. **All stones** that were jumped over are **removed** from the board and returned to the shared stack.

### 4b. Landing Rules

- The landing square must be **empty and on the board**.
- **Exception — goal lines**: the ball may land **off-board**, but **only** if it exits behind a goal line (past the first or last column). This counts as a goal (see Section 5).
- The ball **cannot** exit through the **top or bottom edges** — these are walls.
- If the landing square is **occupied** by another stone, the hop is **invalid**.

### 4c. Chain Jumps

- After landing from a hop, if the ball **can** make another valid hop in **any direction** (including a different direction than the previous hop), it **must** continue jumping.
- The ball **may revisit** squares it has already passed through or landed on during the same chain.
- The chain ends when **no further valid hop** exists from the ball's current position.

### 4d. Goal Scoring During Chains

- If the ball lands on the opponent's goal line (or off-board behind it) at **any point** during a chain, the game **ends immediately** — the current player wins.
- This applies even if further jumps would otherwise be possible.

## 5. Winning

A player wins by moving the ball to or through the **opponent's** goal line:
- **Player 1 wins** if the ball reaches the **last column** or goes off-board to its right.
- **Player 2 wins** if the ball reaches the **first column** or goes off-board to its left.

## 6. Draw Condition

- If **both players skip their turns consecutively** (2 consecutive skipped turns — one per player), the game ends in a **draw**.
- This prevents infinite loops when no stones remain and no jumps are available.

## 7. Edge Cases Summary

| Situation | Resolution |
|---|---|
| Stone stack empty, no valid jump | Turn is skipped |
| Both players skip consecutively | Game ends in a draw |
| Stone placed on a goal line square | Allowed |
| Ball revisits a square during a chain | Allowed |
| Chain reaches opponent's goal line | Immediate win — chain stops |
| Hop would exit top or bottom edge | Invalid — not allowed |
| Hop would exit behind a goal line | Valid — counts as a goal |
| Landing square is occupied | Invalid hop |
