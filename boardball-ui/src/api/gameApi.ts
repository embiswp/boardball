import axios from 'axios'
import type { GameState, PlayerKey, DirectionKey } from '../types/game'

const BASE = '/api/game'

function unwrap<T>(promise: Promise<{ data: T }>): Promise<T> {
  return promise.then(r => r.data)
}

export const startGame = (
  player1: string,
  player2: string,
  rows: number,
  columns: number,
  stackSize: number,
): Promise<GameState> =>
  unwrap(axios.post<GameState>(`${BASE}/start`, { player1, player2, rows, columns, stackSize }))

export const getState = (): Promise<GameState> =>
  unwrap(axios.get<GameState>(`${BASE}/state`))

export const placeStone = (player: PlayerKey, row: number, column: number): Promise<GameState> =>
  unwrap(axios.post<GameState>(`${BASE}/place-stone`, { player, row, column }))

export const jump = (player: PlayerKey, direction: DirectionKey): Promise<GameState> =>
  unwrap(axios.post<GameState>(`${BASE}/jump`, { player, direction }))

export const skipTurn = (player: PlayerKey): Promise<GameState> =>
  unwrap(axios.post<GameState>(`${BASE}/skip`, { player }))
