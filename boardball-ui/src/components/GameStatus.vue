<template>
  <div class="status-bar">
    <template v-if="store.game?.status === 'Won'">
      <div class="result won">🏆 {{ store.winnerName }} wins!</div>
      <button class="new-game" @click="newGame">New Game</button>
    </template>
    <template v-else-if="store.game?.status === 'Draw'">
      <div class="result draw">🤝 Draw!</div>
      <button class="new-game" @click="newGame">New Game</button>
    </template>
    <template v-else>
      <div class="turn-info">
        <span class="player-badge" :class="currentPlayerClass">
          {{ store.currentPlayerName }}'s turn
        </span>
        <span class="stack">🪨 Stack: {{ store.game?.stackCount }}</span>
      </div>
      <div v-if="store.game?.chainInProgress" class="chain-notice">
        Chain jump in progress — pick a direction!
      </div>
      <p v-if="store.error" class="error">{{ store.error }}</p>
    </template>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useGameStore } from '../stores/gameStore'

const store = useGameStore()

const currentPlayerClass = computed(() =>
  store.game?.currentPlayer === 'One' ? 'p1' : 'p2'
)

function newGame() {
  // Reset by forcing the page back to setup — we clear the store game
  store.game = null
  store.error = null
}
</script>

<style scoped>
.status-bar {
  display: flex;
  align-items: center;
  gap: 16px;
  padding: 10px 16px;
  background: #f1f5f9;
  border-radius: 8px;
  flex-wrap: wrap;
}
.turn-info { display: flex; align-items: center; gap: 12px; }
.player-badge {
  padding: 4px 12px;
  border-radius: 20px;
  font-weight: 600;
  font-size: 0.95rem;
}
.p1 { background: #dbeafe; color: #1d4ed8; }
.p2 { background: #fce7f3; color: #be185d; }
.stack { color: #6b7280; font-size: 0.9rem; }
.chain-notice {
  background: #fef3c7;
  border: 1px solid #f59e0b;
  border-radius: 6px;
  padding: 4px 10px;
  font-size: 0.85rem;
  color: #92400e;
}
.result { font-size: 1.3rem; font-weight: 700; }
.won  { color: #15803d; }
.draw { color: #6b7280; }
.new-game {
  padding: 6px 16px;
  background: #2563eb;
  color: white;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-size: 0.9rem;
}
.error { color: #dc2626; font-size: 0.85rem; }
</style>
