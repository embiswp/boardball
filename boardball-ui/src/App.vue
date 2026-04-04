<template>
  <div id="app">
    <GameSetup v-if="!store.game || store.game.status === 'NotStarted'" />
    <GameBoard v-else :game="store.game" />
  </div>
</template>

<script setup lang="ts">
import { useGameStore } from './stores/gameStore'
import GameSetup from './components/GameSetup.vue'
import GameBoard from './components/GameBoard.vue'

const store = useGameStore()
</script>

<style>
* { box-sizing: border-box; margin: 0; padding: 0; }
body {
  font-family: system-ui, sans-serif;
  background: #f8fafc;
  color: #1e293b;
  min-height: 100vh;
}
#app { min-height: 100vh; }
</style>
