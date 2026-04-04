import { defineStore } from 'pinia'
import * as api from '../api/gameApi'
import type { GameState, PlayerKey, DirectionKey } from '../types/game'

export const useGameStore = defineStore('game', {
  state: () => ({
    game: null as GameState | null,
    error: null as string | null,
    loading: false,
  }),

  getters: {
    currentPlayerName: (state): string => {
      if (!state.game) return ''
      return state.game.currentPlayer === 'One'
        ? (state.game.player1Name ?? 'Player 1')
        : (state.game.player2Name ?? 'Player 2')
    },
    winnerName: (state): string => {
      if (!state.game?.winner) return ''
      return state.game.winner === 'One'
        ? (state.game.player1Name ?? 'Player 1')
        : (state.game.player2Name ?? 'Player 2')
    },
  },

  actions: {
    async startGame(
      player1: string,
      player2: string,
      rows: number,
      columns: number,
      stackSize: number,
    ) {
      await this._call(() => api.startGame(player1, player2, rows, columns, stackSize))
    },

    async placeStone(player: PlayerKey, row: number, column: number) {
      await this._call(() => api.placeStone(player, row, column))
    },

    async jump(player: PlayerKey, direction: DirectionKey) {
      await this._call(() => api.jump(player, direction))
    },

    async skipTurn(player: PlayerKey) {
      await this._call(() => api.skipTurn(player))
    },

    clearError() {
      this.error = null
    },

    async _call(fn: () => Promise<GameState>) {
      this.loading = true
      this.error = null
      try {
        this.game = await fn()
      } catch (e: unknown) {
        const axiosError = e as { response?: { data?: { error?: string } } }
        this.error = axiosError?.response?.data?.error ?? 'Unknown error'
      } finally {
        this.loading = false
      }
    },
  },
})
