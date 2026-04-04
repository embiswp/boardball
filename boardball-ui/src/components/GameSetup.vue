<template>
  <div class="setup">
    <h1>Boardball</h1>
    <form @submit.prevent="submit">
      <div class="field">
        <label>Player 1 name</label>
        <input v-model="player1" required placeholder="Player 1" />
      </div>
      <div class="field">
        <label>Player 2 name</label>
        <input v-model="player2" required placeholder="Player 2" />
      </div>
      <div class="field">
        <label>Rows (odd, ≥ 1)</label>
        <input v-model.number="rows" type="number" min="1" step="2" required />
      </div>
      <div class="field">
        <label>Columns (odd, ≥ 5)</label>
        <input v-model.number="columns" type="number" min="5" step="2" required />
      </div>
      <div class="field">
        <label>Stones in stack</label>
        <input v-model.number="stackSize" type="number" min="1" required />
      </div>
      <p v-if="store.error" class="error">{{ store.error }}</p>
      <button type="submit" :disabled="store.loading">Start Game</button>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useGameStore } from '../stores/gameStore'

const store = useGameStore()
const player1  = ref('Player 1')
const player2  = ref('Player 2')
const rows     = ref(15)
const columns  = ref(19)
const stackSize = ref(30)

async function submit() {
  await store.startGame(player1.value, player2.value, rows.value, columns.value, stackSize.value)
}
</script>

<style scoped>
.setup {
  max-width: 360px;
  margin: 60px auto;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 16px;
}
h1 { font-size: 2rem; margin-bottom: 8px; }
form { width: 100%; display: flex; flex-direction: column; gap: 12px; }
.field { display: flex; flex-direction: column; gap: 4px; }
label { font-size: 0.85rem; color: #666; }
input {
  padding: 8px;
  border: 1px solid #ccc;
  border-radius: 6px;
  font-size: 1rem;
}
button {
  padding: 10px;
  background: #2563eb;
  color: white;
  border: none;
  border-radius: 6px;
  font-size: 1rem;
  cursor: pointer;
}
button:disabled { opacity: 0.6; cursor: not-allowed; }
.error { color: #dc2626; font-size: 0.85rem; }
</style>
