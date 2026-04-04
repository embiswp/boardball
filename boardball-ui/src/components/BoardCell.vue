<template>
  <div
    class="cell"
    :class="[cellClass, { clickable: isClickable }]"
    :title="title"
    @click="onClick"
  >
    <span v-if="hasBall" class="ball">⚽</span>
    <span v-else-if="hasStone" class="stone"></span>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'

const props = defineProps<{
  row: number
  column: number
  hasBall: boolean
  hasStone: boolean
  isGoalLeft: boolean
  isGoalRight: boolean
  canPlace: boolean
}>()

const emit = defineEmits<{
  (e: 'cell-clicked', row: number, column: number): void
}>()

const isClickable = computed(() => props.canPlace && !props.hasBall && !props.hasStone)

const cellClass = computed(() => {
  if (props.hasBall) return 'ball-cell'
  if (props.hasStone) return 'stone-cell'
  if (props.isGoalLeft) return 'goal-left'
  if (props.isGoalRight) return 'goal-right'
  return 'empty'
})

const title = computed(() => {
  if (props.hasBall) return 'Ball'
  if (props.hasStone) return 'Stone'
  if (props.isGoalLeft) return 'Player 2 goal'
  if (props.isGoalRight) return 'Player 1 goal'
  return `(${props.row}, ${props.column})`
})

function onClick() {
  if (isClickable.value) emit('cell-clicked', props.row, props.column)
}
</script>

<style scoped>
.cell {
  width: 36px;
  height: 36px;
  border: 1px solid #d1d5db;
  display: flex;
  align-items: center;
  justify-content: center;
  background: #f9fafb;
  transition: background 0.15s;
  user-select: none;
}
.cell.clickable {
  cursor: pointer;
}
.cell.clickable:hover {
  background: #dbeafe;
}
.ball-cell { background: #fef3c7; }
.stone-cell { background: #e5e7eb; }
.goal-left  { background: #fce7f3; }
.goal-right { background: #dcfce7; }
.ball { font-size: 20px; line-height: 1; }
.stone {
  width: 22px;
  height: 22px;
  border-radius: 50%;
  background: #374151;
  display: block;
}
</style>
