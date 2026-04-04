<template>
  <div class="game-view">
    <GameStatus />

    <div class="board-and-actions">
      <div class="board-wrap">
        <!-- Column labels -->
        <div class="col-labels">
          <div class="row-spacer"></div>
          <div v-for="c in game.columns" :key="c" class="col-label">{{ c }}</div>
        </div>

        <div v-for="r in game.rows" :key="r" class="board-row">
          <div class="row-label">{{ r }}</div>
          <BoardCell
            v-for="c in game.columns"
            :key="c"
            :row="r"
            :column="c"
            :hasBall="game.ball.row === r && game.ball.column === c"
            :hasStone="hasStone(r, c)"
            :isGoalLeft="c === 1"
            :isGoalRight="c === game.columns"
            :canPlace="game.canPlaceStone && game.status === 'InProgress'"
            @cell-clicked="onCellClicked"
          />
        </div>
      </div>

      <ActionPanel />
    </div>

    <div class="legend">
      <span class="legend-item goal-left-swatch">Player 2 goal (left)</span>
      <span class="legend-item goal-right-swatch">Player 1 goal (right)</span>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { GameState, PlayerKey } from '../types/game'
import { useGameStore } from '../stores/gameStore'
import BoardCell from './BoardCell.vue'
import GameStatus from './GameStatus.vue'
import ActionPanel from './ActionPanel.vue'

const props = defineProps<{ game: GameState }>()
const store = useGameStore()

function hasStone(row: number, col: number) {
  return store.game?.stones.some(s => s.row === row && s.column === col) ?? false
}

async function onCellClicked(row: number, column: number) {
  if (!store.game?.canPlaceStone) return
  const player = store.game.currentPlayer as PlayerKey
  await store.placeStone(player, row, column)
}
</script>

<style scoped>
.game-view {
  display: flex;
  flex-direction: column;
  gap: 12px;
  padding: 16px;
  max-width: 100%;
  overflow-x: auto;
}
.board-and-actions {
  display: flex;
  gap: 16px;
  align-items: flex-start;
}
.board-wrap {
  display: flex;
  flex-direction: column;
  gap: 0;
}
.col-labels {
  display: flex;
  gap: 0;
  margin-bottom: 2px;
}
.row-spacer { width: 24px; }
.col-label {
  width: 36px;
  text-align: center;
  font-size: 0.7rem;
  color: #9ca3af;
}
.board-row { display: flex; align-items: center; gap: 0; }
.row-label {
  width: 24px;
  text-align: right;
  font-size: 0.7rem;
  color: #9ca3af;
  padding-right: 4px;
}
.legend {
  display: flex;
  gap: 16px;
  font-size: 0.8rem;
}
.legend-item {
  padding: 3px 8px;
  border-radius: 4px;
}
.goal-left-swatch  { background: #fce7f3; }
.goal-right-swatch { background: #dcfce7; }
</style>
