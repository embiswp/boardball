<template>
  <div class="action-panel">
    <!-- Jump direction picker -->
    <div v-if="store.game?.canJump" class="jump-section">
      <div class="label">Jump direction:</div>
      <div class="dir-grid">
        <button
          v-for="dir in allDirections"
          :key="dir"
          class="dir-btn"
          :class="{ available: isAvailable(dir), unavailable: !isAvailable(dir) }"
          :disabled="!isAvailable(dir)"
          @click="doJump(dir)"
          :title="dir"
        >
          {{ arrow(dir) }}
        </button>
      </div>
    </div>

    <!-- Skip turn -->
    <button
      v-if="store.game?.mustSkip"
      class="skip-btn"
      :disabled="store.loading"
      @click="doSkip"
    >
      Skip Turn
    </button>
  </div>
</template>

<script setup lang="ts">
import { useGameStore } from '../stores/gameStore'
import type { DirectionKey, PlayerKey } from '../types/game'

const store = useGameStore()

const allDirections: DirectionKey[] = [
  'UpLeft', 'Up', 'UpRight',
  'Left', 'Right',
  'DownLeft', 'Down', 'DownRight',
]

const arrowMap: Record<DirectionKey, string> = {
  Up: '↑', Down: '↓', Left: '←', Right: '→',
  UpLeft: '↖', UpRight: '↗', DownLeft: '↙', DownRight: '↘',
}

function arrow(dir: DirectionKey) { return arrowMap[dir] }

function isAvailable(dir: DirectionKey) {
  return store.game?.availableJumpDirections.includes(dir) ?? false
}

async function doJump(dir: DirectionKey) {
  const player = store.game?.currentPlayer as PlayerKey
  await store.jump(player, dir)
}

async function doSkip() {
  const player = store.game?.currentPlayer as PlayerKey
  await store.skipTurn(player)
}
</script>

<style scoped>
.action-panel {
  display: flex;
  flex-direction: column;
  gap: 12px;
  padding: 12px;
  background: #f8fafc;
  border-radius: 8px;
  min-width: 160px;
}
.label { font-size: 0.85rem; color: #6b7280; margin-bottom: 4px; }
.dir-grid {
  display: grid;
  grid-template-columns: repeat(3, 40px);
  grid-template-rows: repeat(3, 40px);
  gap: 4px;
}
/* Position buttons: row1=UpLeft,Up,UpRight / row2=Left,_,Right / row3=DownLeft,Down,DownRight */
.dir-btn {
  width: 40px;
  height: 40px;
  font-size: 1.2rem;
  border: 1px solid #d1d5db;
  border-radius: 6px;
  cursor: pointer;
  background: white;
}
.dir-btn.available {
  background: #dbeafe;
  border-color: #2563eb;
  color: #1d4ed8;
}
.dir-btn.available:hover { background: #bfdbfe; }
.dir-btn.unavailable {
  opacity: 0.3;
  cursor: not-allowed;
}
/* Hide the center cell (no center button) */
.dir-btn:nth-child(5) { visibility: hidden; }
.skip-btn {
  padding: 8px 16px;
  background: #f59e0b;
  color: white;
  border: none;
  border-radius: 6px;
  font-size: 0.9rem;
  cursor: pointer;
}
.skip-btn:disabled { opacity: 0.6; cursor: not-allowed; }
</style>
