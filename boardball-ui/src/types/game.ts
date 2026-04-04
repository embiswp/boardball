export type PlayerKey = 'One' | 'Two'
export type GameStatusKey = 'NotStarted' | 'InProgress' | 'Won' | 'Draw'
export type DirectionKey =
  | 'Up' | 'Down' | 'Left' | 'Right'
  | 'UpLeft' | 'UpRight' | 'DownLeft' | 'DownRight'

export interface GameState {
  status: GameStatusKey
  winner: PlayerKey | null
  currentPlayer: PlayerKey | null
  player1Name: string | null
  player2Name: string | null
  rows: number
  columns: number
  stackCount: number
  ball: { row: number; column: number }
  stones: { row: number; column: number }[]
  chainInProgress: boolean
  availableJumpDirections: DirectionKey[]
  canPlaceStone: boolean
  canJump: boolean
  mustSkip: boolean
  consecutiveSkips: number
}
